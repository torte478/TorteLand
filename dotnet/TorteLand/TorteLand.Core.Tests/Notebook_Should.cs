using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Notebooks;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class Notebook_Should
{
    [Test]
    public void AddElemToEnd_WhenNotebookIsNotEmpty()
    {
        var notebook = new Notebook(Maybe.None<List<string>>())
                       {
                           { "a", Maybe.None<ResolvedSegment>() },
                           { "b", Maybe.None<ResolvedSegment>() }
                       };

        var segment = new Segment(0, 0, 1);
        var halfSegment = new ResolvedSegment(segment, true);

        var actual = notebook.Add("b", Maybe.Some(halfSegment));

        var index = actual.Match(_ => _, _ => -1);
        Assert.That(index, Is.EqualTo(1));
    }

    [Test]
    public void AddElemToBegin_WhenNotebookIsNotEmpty()
    {
        var notebook = new Notebook(Maybe.None<List<string>>())
                       {
                           { "a", Maybe.None<ResolvedSegment>() },
                           { "b", Maybe.None<ResolvedSegment>() },
                           { "b", Maybe.Some(new ResolvedSegment(new Segment(0, 0, 1), true)) },
                           { "c", Maybe.None<ResolvedSegment>() },
                           { "c", Maybe.Some(new ResolvedSegment(new Segment(0, 1, 3), false)) },
                       };

        var segment = new Segment(0, 0, 1);
        var halfSegment = new ResolvedSegment(segment, false);

        var actual = notebook.Add("c", Maybe.Some(halfSegment));

        var index = actual.Match(_ => _, _ => -1);
        Assert.That(index, Is.EqualTo(0));
    }
}