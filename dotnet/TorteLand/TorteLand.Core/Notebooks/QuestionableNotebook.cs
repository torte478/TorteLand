using System;
using System.Collections;
using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Core.Extensions;
using TorteLand.Extensions;

namespace TorteLand.Core.Notebooks;

internal sealed class QuestionableNotebook : IQuestionableNotebook
{
    private readonly Dictionary<Guid, (Added Added, Segment Segment)> _transactions;
    private readonly INotebook _origin;

    // ReSharper disable once UnusedMember.Global
    public QuestionableNotebook(INotebook origin)
    {
        _origin = origin;
        _transactions = new Dictionary<Guid, (Added Added, Segment Segment)>();
    }

    private QuestionableNotebook(
        INotebook origin,
        Dictionary<Guid, (Added Added, Segment Segment)> transactions)
    {
        _origin = origin;
        _transactions = transactions;
    }

    public Page<Unique<Note>> All(Maybe<Pagination> pagination)
        => _origin.All(pagination);

    public AddNotesIteration Create(Added added)
        => Add(
            added,
            Maybe.None<ResolvedSegment>(),
            _ => StartTransaction(_, added));

    public AddNotesIteration Create(Guid id, bool isRight)
    {
        var transaction = _transactions[id];
        var segment = new ResolvedSegment(transaction.Segment, isRight);

        return Add(
            transaction.Added,
            Maybe.Some(segment),
            _ => UpdateTransaction(_, id));
    }

    public IQuestionableNotebook Delete(int key)
        => _origin
           .Delete(key)
           .Wrap<QuestionableNotebook>();

    public (IQuestionableNotebook Notebook, Either<byte, int> Result) Increment(int key)
    {
        var (notebook, result) = _origin.Increment(key);

        return (notebook.Wrap<QuestionableNotebook>(), result);
    }
    
    public (IQuestionableNotebook Notebook, Either<byte, int> Result) Decrement(int key)
    {
        var (notebook, result) = _origin.Decrement(key);

        return (notebook.Wrap<QuestionableNotebook>(), result);
    }

    public IQuestionableNotebook Update(int key, string name)
        => _origin
           .Update(key, name)
           .Wrap<QuestionableNotebook>();

    public Maybe<Note> Read(int key)
        => _origin
           .Read(key);

    public IEnumerator<Unique<Note>> GetEnumerator()
        => _origin.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private AddNotesIteration Add(
        Added added,
        Maybe<ResolvedSegment> segment,
        Func<Segment, AddNotesIteration> onRight)
        => _origin
           .Create(added, segment)
           .Match(
               CompleteTransaction,
               onRight);

    private static AddNotesIteration CompleteTransaction(AddNotesResult result)
        => new(
            Notebook: result.Notebook.Wrap<QuestionableNotebook>(),
            Result: result.Indices._(Either.Left<IReadOnlyCollection<int>, Question>));

    private AddNotesIteration UpdateTransaction(Segment segment, Guid id)
        => _transactions[id]
           .Added
           ._(_ => (_, segment))
           ._(_ => _transactions.SetImmutable(id, _))
           ._(_ => new QuestionableNotebook(_origin, _))
           ._(BuildTransaction, id, segment);

    private AddNotesIteration StartTransaction(Segment segment, Added added)
    {
        var key = Guid.NewGuid();
        var notebook = new QuestionableNotebook(
            origin: _origin,
            transactions: _transactions.AddImmutable(key, (added, segment)));

        return BuildTransaction(notebook, key, segment);
    }

    private AddNotesIteration BuildTransaction(IQuestionableNotebook notebook, Guid id, Segment segment)
        => segment
           .Border
           ._(_origin.Read)
           .ToSome()
           .Text
           ._(_ => new Question(id, _))
           ._(Either.Right<IReadOnlyCollection<int>, Question>)
           ._(_ => new AddNotesIteration(notebook, _));
}