using FakeItEasy;
using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Notebooks;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class QuestionableNotebook_Should
{
    [Test]
    public void CopyTransactionInfo_AfterClone()
    {
        var origin = A.Fake<INotebook>();
        A.CallTo(() => origin.Create(A<IReadOnlyCollection<string>>._, A<Maybe<ResolvedSegment>>._))
         .Returns(new Right<AddNotesResult, Segment>(A.Fake<Segment>()));
        A.CallTo(() => origin.Read(A<int>._))
         .Returns(Maybe.Some<Note>(new Note(string.Empty, 0)));

        var notebook = new QuestionableNotebook(origin);

        var added = notebook.Create(new[] { "value" });
        var question = added.ToRight();

        var clone = notebook.Clone();

        Assert.DoesNotThrow(() => clone.Create(question.Id, true));
    }
}