using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

using Added = System.Collections.Generic.IReadOnlyCollection<int>;

namespace TorteLand.Core.Notebooks;

internal sealed class QuestionableNotebook : IQuestionableNotebook
{
    private readonly Dictionary<Guid, (IReadOnlyCollection<string> Text, Segment Segment)> _transactions = new();

    private readonly IAsyncNotebook _origin;

    public QuestionableNotebook(IAsyncNotebook origin)
    {
        _origin = origin;
    }

    public IAsyncEnumerable<Unique<Note>> All(CancellationToken token)
        => _origin.All(token);

    public Task<Either<Added, Question>> Add(IReadOnlyCollection<string> values, CancellationToken token)
        => Add(
            values,
            Maybe.None<ResolvedSegment>(),
            (s, t) => StartTransaction(s, values, t),
            token);

    public async Task<Either<Added, Question>> Add(Guid id, bool isRight, CancellationToken token)
    {
        var transaction = _transactions[id];
        var segment = new ResolvedSegment(transaction.Segment, isRight);

        return await Add(
                   transaction.Text,
                   Maybe.Some(segment),
                   (s, t) => UpdateTransaction(s, id, t),
                   token);
    }

    public async Task Delete(int key, CancellationToken token)
    {
        await _origin.Delete(key, token);
        _transactions.Clear();
    }

    public async Task DeleteAll(CancellationToken token)
    {
        await _origin.DeleteAll(token);
        _transactions.Clear();
    }

    public async Task Rename(int key, string text, CancellationToken token)
    {
        await _origin.Rename(key, text, token);
        _transactions.Clear();
    }

    private async Task<Either<Added, Question>> Add(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment,
        Func<Segment, CancellationToken, Task<Either<Added, Question>>> onRight,
        CancellationToken token)
    {
        var result = await _origin.Add(values, segment, token);

        return await result.MatchAsync(
                   CompleteTransaction,
                   _ => onRight(_, token));
    }

    private Task<Either<Added, Question>> CompleteTransaction(Added keys)
    {
        _transactions.Clear();
        return keys._(Either.Left<Added, Question>)._(Task.FromResult);
    }

    private Task<Either<Added, Question>> UpdateTransaction(Segment segment, Guid id, CancellationToken token)
    {
        var transaction = _transactions[id];
        _transactions[id] = (transaction.Text, segment);

        return BuildTransaction(id, segment, token);
    }

    private Task<Either<Added, Question>> StartTransaction(Segment segment, IReadOnlyCollection<string> values, CancellationToken token)
    {
        var key = Guid.NewGuid();
        _transactions.Add(key, (values, segment));

        return BuildTransaction(key, segment, token);
    }

    private async Task<Either<Added, Question>> BuildTransaction(Guid id, Segment segment, CancellationToken token)
    {
        var note = await _origin.ToNote(segment.Border, token);

        return new Question(id, note.Text)
            ._(Either.Right<Added, Question>);
    }
}