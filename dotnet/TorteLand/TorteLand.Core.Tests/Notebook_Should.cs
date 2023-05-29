using NUnit.Framework;
using NUnit.Framework.Constraints;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Notebooks;
using TorteLand.Extensions;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class Notebook_Should
{
    [Test]
    public void SaveOrder_OnEnumerate()
    {
        var notebook = Create(new[] { "2", "1" });

        var actual = notebook.ToArray();

        Assert.That(actual[0].Value.Text, Is.EqualTo("2"));
    }

    [Test]
    public void SaveOrder_OnAddToEmpty()
    {
        var notebook = Create(Array.Empty<string>());
        var result = notebook.Create(new[] { "1", "2" }.Wrap<Added>(), Maybe.None<ResolvedSegment>());

        var actual = result.ToLeft().Notebook.ToArray();
        Assert.That(actual[0].Value.Text, Is.EqualTo("1"));
    }

    [Test]
    public void SaveOrder_AfterRename()
    {
        var notebook = Create(new[] { "2", "1", "0" });

        var updated = notebook.Update(0, "renamed");

        var actual = updated.Select(_ => _.Value.Weight);
        Assert.That(actual.SequenceEqual(new[] { 2, 1, 0}), Is.True);
    }

    [Test]
    public void ShiftInterval_WhenInsertAfterOrigin()
    {
        var notebook = Create(new[] { "a", "b", "c" });

        var actual = notebook.Create(
            new Added(
                "Z".AsArray(),
                false,
                Maybe.Some(0),
                Direction.After),
            Maybe.None<ResolvedSegment>());

        Assert.That(actual.ToRight().Begin, Is.EqualTo(1));
    }

    [Test]
    public void ShiftInterval_WhenAddAfterMiddle()
    {
        var notebook = Create(new[] { "a", "b", "c", "d", "e" });

        var result = notebook.Create(
            added: new Added(
                Values: new[] { "Z" },
                Exact: false,
                Origin: 1._(Maybe.Some),
                Direction.After),
            segment: new ResolvedSegment(
                    Segment: new Segment(2,  3, 6),
                    IsGreater: false)
                ._(Maybe.Some));
        
        Assert.That(result.ToRight().Begin, Is.EqualTo(4));
    }

    [TestCaseSource(nameof(_testCaseSource))]
    public void CorrectInsertElements(
        string name,
        IReadOnlyCollection<string> init,
        Added added,
        Maybe<ResolvedSegment> segment,
        IReadOnlyCollection<string> expected)
    {
        var notebook = Create(init);
        var result = notebook.Create(added, segment);

        var actual = result.ToLeft().Notebook.Select(_ => _.Value.Text).ToArray();
        Assert.That(actual, new EqualConstraint(expected));
    }

    private static INotebook Create(IReadOnlyCollection<string> notes)
        => notes
           .Select(_ => (_, (byte)0))
           .ToArray()
           ._(_ => _ as IReadOnlyCollection<(string, byte)>)
           ._(Maybe.Some)
           ._(_ => new Notebook(_, default));

    // TODO : refactor
    private static object[] _testCaseSource =
    {
        new object[]
        {
            "insert to top",
            new[] { "a" },
            new[] { "Z" }.Wrap<Added>(),
            new ResolvedSegment(new Segment(0, 0, 1), true)._(Maybe.Some),
            new[] { "Z", "a" }
        },
        new object[]
        {
            "insert to middle",
            new[] { "b", "a" },
            new[] { "Z" }.Wrap<Added>(),
            new ResolvedSegment(new Segment(0, 0, 1), false)._(Maybe.Some),
            new[] { "b", "Z", "a" }
        },
        new object[]
        {
            "insert range to middle",
            new[] { "d", "c", "b", "a" },
            new[] { "Z", "Y" }.Wrap<Added>(),
            new ResolvedSegment(new Segment(3, 3, 4), true)._(Maybe.Some),
            new[] { "d", "c", "b", "Z", "Y", "a" }
        },
        new object[]
        {
            "insert exact after top",
            new[] { "a", "b" },
            new Added(
                new[] { "Z" },
                true,
                Maybe.Some(0),
                Direction.After),
            Maybe.None<ResolvedSegment>(),
            new[] { "a", "Z", "b"}
        },
        new object[]
        {
            "insert exact before bottom",
            new[] { "a", "b" },
            new Added(
                new[] { "Z" },
                true,
                Maybe.Some(1),
                Direction.Before),
            Maybe.None<ResolvedSegment>(),
            new[] { "a", "Z", "b" }
        }
    };
}