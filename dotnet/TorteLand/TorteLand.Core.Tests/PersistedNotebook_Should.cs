using FakeItEasy;
using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Contracts;
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
        var notebook = new PersistedNotebook(A.Fake<IStorage>(), factory);

        await notebook.All(Maybe.None<Pagination>(), default);
        await notebook.All(Maybe.None<Pagination>(), default);

        A.CallTo(() => factory.Create(A<IReadOnlyCollection<Note>>._))
         .MustHaveHappenedOnceExactly();
    }
}