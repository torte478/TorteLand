using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

using Added = System.Collections.Generic.IReadOnlyCollection<int>;

namespace TorteLand.Core.Notebooks;

internal sealed class QuestionableNotebook : IQuestionableNotebook
{
    private readonly Dictionary<Guid, (IReadOnlyCollection<string> Text, Segment Segment)> _transactions;

    private readonly INotebook _origin;

    public QuestionableNotebook(INotebook origin)
    {
        _origin = origin;
        _transactions = new Dictionary<Guid, (IReadOnlyCollection<string> Text, Segment Segment)>();
    }

    private QuestionableNotebook(
        INotebook origin,
        Dictionary<Guid, (IReadOnlyCollection<string> Text, Segment Segment)> transaction)
    {
        _origin = origin;
        _transactions = transaction;
    }

    public Page<Unique<Note>> All(Maybe<Pagination> pagination)
        => _origin.All(pagination);

    public Either<Added, Question> Create(IReadOnlyCollection<string> values)
        => Add(
            values,
            Maybe.None<ResolvedSegment>(),
            _ => StartTransaction(_, values));

    public Either<Added, Question> Create(Guid id, bool isRight)
    {
        var transaction = _transactions[id];
        var segment = new ResolvedSegment(transaction.Segment, isRight);

        return Add(
            transaction.Text,
            Maybe.Some(segment),
            _ => UpdateTransaction(_, id));
    }

    public Note Delete(int key)
    {
        var note = _origin.Delete(key);
        _transactions.Clear();
        return note;
    }

    public IQuestionableNotebook Clone()
    {
        var origin = _origin.Clone();
        var transaction = _transactions.ToDictionary(
            x => x.Key,
            x => x.Value);
        return new QuestionableNotebook(origin, transaction);
    }

    public void Update(int key, string name)
    {
        _origin.Update(key, name);
        _transactions.Clear();
    }

    public Maybe<string> Read(int key)
        => _origin.Read(key);

    public IEnumerator<Unique<Note>> GetEnumerator()
        => _origin.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private Either<Added, Question> Add(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment,
        Func<Segment, Either<Added, Question>> onRight)
    {
        var result = _origin.Add(values, segment);

        return result.Match(
                   CompleteTransaction,
                   onRight);
    }

    private Either<Added, Question> CompleteTransaction(Added keys)
    {
        _transactions.Clear();
        return keys._(Either.Left<Added, Question>);
    }

    private Either<Added, Question> UpdateTransaction(Segment segment, Guid id)
    {
        var transaction = _transactions[id];
        _transactions[id] = (transaction.Text, segment);

        return BuildTransaction(id, segment);
    }

    private Either<Added, Question> StartTransaction(Segment segment, IReadOnlyCollection<string> values)
    {
        var key = Guid.NewGuid();
        _transactions.Add(key, (values, segment));

        return BuildTransaction(key, segment);
    }

    private Either<Added, Question> BuildTransaction(Guid id, Segment segment)
    {
        var note = _origin.ToNote(segment.Border);

        return new Question(id, note.Text)
            ._(Either.Right<Added, Question>);
    }
}