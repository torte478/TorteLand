using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Notebooks;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class Notebook_Should
{
    [Test]
    public void GetDescentOrder_OnEnumerate()
    {
        var notebook = Create(new[] { "2", "1" });

        var actual = notebook.ToArray();

        Assert.That(actual[0].Value.Text, Is.EqualTo("2"));
    }

    [Test]
    public void GetDescentOrder_OnAddToEmpty()
    {
        var notebook = Create(Array.Empty<string>());
        notebook.Add(new[] { "2", "1" }, Maybe.None<ResolvedSegment>());

        var actual = notebook.ToArray();
        Assert.That(actual[0].Value.Text, Is.EqualTo("2"));
    }

    [Test]
    public void SaveOrder_AfterRename()
    {
        var notebook = Create(Array.Empty<string>());
        notebook.Add(new[] { "2", "1", "0" }, Maybe.None<ResolvedSegment>());

        notebook.Update(0, "renamed");

        var actual = notebook.Select(_ => _.Value.Weight);
        Assert.That(actual.SequenceEqual(new[] { 2, 1, 0}), Is.True);
    }

    [Test]
    public void SaveOrder_AfterClone()
    {
        var notebook = Create(Array.Empty<string>());
        notebook.Add(new[] { "2", "1", "0" }, Maybe.None<ResolvedSegment>());

        var cloned = notebook.Clone();

        var actual = cloned.Select(_ => _.Value.Text).ToArray();
        Assert.That(actual[0], Is.EqualTo("2"));
    }

    [Test]
    [TestCaseSource(nameof(_testCaseSource))]
    public void CorrectInsertElements(
        string name,
        IReadOnlyCollection<string> init,
        IReadOnlyCollection<string> toAdd,
        ResolvedSegment segment,
        IReadOnlyCollection<string> expected)
    {
        var notebook = Create(init);
        notebook.Add(toAdd, Maybe.Some(segment));

        var actual = notebook.Select(_ => _.Value.Text);
        Assert.That(actual.SequenceEqual(expected), Is.True);
    }

    private static INotebook Create(IReadOnlyCollection<string> notes)
        => notes._(Maybe.Some)._(_ => new Notebook(_));

    private static object[] _testCaseSource
        = {
              new object[]
              {
                  "insert to end",
                  new[] { "a" },
                  new[] { "Z" },
                  new ResolvedSegment(new Segment(0, 0, 1), true),
                  new[] { "Z", "a" }
              },
              new object[]
              {
                  "insert to begin",
                  new[] { "b", "a" },
                  new[] { "Z" },
                  new ResolvedSegment(new Segment(0, 0, 1), false),
                  new[] { "b", "a", "Z" }
              },
                new object[]
                {
                    "insert range to middle",
                    new[] { "d", "c", "b", "a"},
                    new[] { "Z", "Y" },
                    new ResolvedSegment(new Segment(3, 3, 4), false),
                    new[] { "d", "Z", "Y", "c", "b", "a"}
                }
          };
}