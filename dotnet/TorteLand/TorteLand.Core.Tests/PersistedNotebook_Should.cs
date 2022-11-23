using FakeItEasy;
using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
using TorteLand.Core.Storage;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class PersistedNotebook_Should
{
    [Test]
    public async Task CreateOriginOnce_AfterFirstCall()
    {
        var factory = A.Fake<IQuestionableNotebookFactory>();
        var origin = new Left<IQuestionableNotebookFactory, IQuestionableNotebook>(factory);
        var notebook = new PersistedNotebook(A.Fake<IStorage>(), origin);

        await notebook.All(Maybe.None<Pagination>(), default);
        await notebook.All(Maybe.None<Pagination>(), default);

        A.CallTo(() => factory.Create(A<IReadOnlyCollection<Note>>._))
         .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task SaveOrigin_AfterCreateCall()
    {
        var origin = A.Fake<IQuestionableNotebook>();
        A.CallTo(() => origin.Clone())
         .Returns(A.Fake<IQuestionableNotebook>());
        var right = new Right<IQuestionableNotebookFactory, IQuestionableNotebook>(origin);
        var notebook = new PersistedNotebook(A.Fake<IStorage>(), right);

        await notebook.Create(new[] { "note" }, default);
        await notebook.Create(new[] { "note" }, default);

        A.CallTo(() => origin.Clone())
         .MustHaveHappenedOnceExactly();
    }
}