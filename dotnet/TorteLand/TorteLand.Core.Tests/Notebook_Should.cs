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
        Assert.That(actual, Is.EquivalentTo(expected), () => name);
    }

    private static object[] _testCaseSource
        = {
              new object[]
              {
                  "insert to end",
                  new[] { "a" },
                  new[] { "Z" },
                  new ResolvedSegment(new Segment(0, 0, 1), true),
                  new[] { "a", "Z" }
              },
              new object[]
              {
                  "insert to begin",
                  new[] { "a", "b" },
                  new[] { "Z" },
                  new ResolvedSegment(new Segment(0, 0, 1), false),
                  new[] { "Z", "a", "b" }
              },
                new object[]
                {
                    "insert range to middle",
                    new[] { "a", "b", "c", "d"},
                    new[] { "Y", "Z" },
                    new ResolvedSegment(new Segment(3, 3, 4), false),
                    new[] { "a", "b", "c", "Y", "Z", "d"}
                }
          };
}