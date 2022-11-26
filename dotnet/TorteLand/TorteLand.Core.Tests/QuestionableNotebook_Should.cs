using FakeItEasy;
using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;
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
        A.CallTo(() => origin.Add(A<IReadOnlyCollection<string>>._, A<Maybe<ResolvedSegment>>._))
         .Returns(new Right<IReadOnlyCollection<int>, Segment>(A.Fake<Segment>()));

        var notebook = new QuestionableNotebook(origin);

        var added = notebook.Create(new[] { "value" });
        var question = added.ToRight<IReadOnlyCollection<int>, Question>();

        var clone = notebook.Clone();

        Assert.DoesNotThrow(() => clone.Create(question.Id, true));
    }
}