using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Notebooks;

internal sealed class TransactionNotebook : ITransactionNotebook
{
    private readonly Dictionary<Guid, (string Text, Segment Segment)> _transactions;

    private readonly IAsyncNotebook _origin;

    public TransactionNotebook(IAsyncNotebook origin)
    {
        _origin = origin;
        _transactions = new Dictionary<Guid, (string Text, Segment Segment)>();
    }

    public IAsyncEnumerable<Unique<Note>> All(CancellationToken token)
        => _origin.All(token);

    public Task<Either<int, Transaction>> Add(string value, CancellationToken token)
        => Add(
            value,
            Maybe.None<ResolvedSegment>(),
            (s, t) => StartTransaction(s, value, t),
            token);

    public async Task<Either<int, Transaction>> Add(Guid id, bool isRight, CancellationToken token)
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

    private async Task<Either<int, Transaction>> Add(
        string value,
        Maybe<ResolvedSegment> segment,
        Func<Segment, CancellationToken, Task<Either<int, Transaction>>> onRight,
        CancellationToken token)
    {
        var result = await _origin.Add(value, segment, token);

        return await result.MatchAsync(
                   CompleteTransaction,
                   _ => onRight(_, token));
    }

    private Task<Either<int, Transaction>> CompleteTransaction(int key)
    {
        _transactions.Clear();
        return key._(Either.Left<int, Transaction>)._(Task.FromResult);
    }

    private Task<Either<int, Transaction>> UpdateTransaction(Segment segment, Guid id, CancellationToken token)
    {
        var transaction = _transactions[id];
        _transactions[id] = (transaction.Text, segment);

        return BuildTransaction(id, segment, token);
    }

    private Task<Either<int, Transaction>> StartTransaction(Segment segment, string value, CancellationToken token)
    {
        var key = Guid.NewGuid();
        _transactions.Add(key, (value, segment));

        return BuildTransaction(key, segment, token);
    }

    private async Task<Either<int, Transaction>> BuildTransaction(Guid id, Segment segment, CancellationToken token)
    {
        var note = await _origin.ToNote(segment.Border, token);

        return new Transaction(id, note.Text)
            ._(Either.Right<int, Transaction>);
    }
}