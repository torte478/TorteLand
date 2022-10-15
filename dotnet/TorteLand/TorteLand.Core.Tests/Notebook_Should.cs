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
        var notebook = new Notebook(Maybe.Some(new List<string> { "1", "2" }));

        var actual = notebook.ToArray();

        Assert.That(actual[0].Value.Text, Is.EqualTo("2"));
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
        var notebook = new Notebook(Maybe.Some(init.ToList()))
                       {
                           { toAdd, Maybe.Some(segment) }
                       };

        var actual = notebook.Select(_ => _.Value.Text);
        Assert.That(actual.SequenceEqual(expected), Is.True);
    }

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
                  new[] { "a", "b" },
                  new[] { "Z" },
                  new ResolvedSegment(new Segment(0, 0, 1), false),
                  new[] { "b", "a", "Z" }
              },
                new object[]
                {
                    "insert range to middle",
                    new[] { "a", "b", "c", "d"},
                    new[] { "Y", "Z" },
                    new ResolvedSegment(new Segment(3, 3, 4), false),
                    new[] { "d", "Z", "Y", "c", "b", "a"}
                }
          };
}