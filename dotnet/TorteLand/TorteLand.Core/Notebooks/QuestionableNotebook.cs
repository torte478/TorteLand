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

// TODO: to immutable
internal sealed class QuestionableNotebook : IQuestionableNotebook
{
    private readonly Dictionary<Guid, (IReadOnlyCollection<string> Text, Segment Segment)> _transactions;

    private INotebook _origin;

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

    public void Delete(int key)
    {
        _origin = _origin.Delete(key);
        _transactions.Clear();
    }

    public IQuestionableNotebook Clone()
    {
        var transaction = _transactions.ToDictionary(
            x => x.Key,
            x => x.Value);
        return new QuestionableNotebook(_origin, transaction);
    }

    public void Update(int key, string name)
    {
        _origin = _origin.Update(key, name);
        _transactions.Clear();
    }

    public Maybe<string> Read(int key)
        => _origin
           .Read(key)
           .Select(_ => _.Text);

    public IEnumerator<Unique<Note>> GetEnumerator()
        => _origin.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private Either<Added, Question> Add(
        IReadOnlyCollection<string> values,
        Maybe<ResolvedSegment> segment,
        Func<Segment, Either<Added, Question>> onRight)
        => _origin
           .Create(values, segment)
           .Match(
               CompleteTransaction,
               onRight);

    private Either<Added, Question> CompleteTransaction(AddNotesResult result)
    {
        _transactions.Clear();
        _origin = result.Notebook;
        return result.Indices._(Either.Left<Added, Question>);
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
        var note = _origin.Read(segment.Border);

        var text = note.ToSome().Text;

        return new Question(id, text)
            ._(Either.Right<Added, Question>);
    }
}