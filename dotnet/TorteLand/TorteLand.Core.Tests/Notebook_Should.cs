using NUnit.Framework;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Notebooks;

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class Notebook_Should
{
    [Test]
    public void Foo_Bar()
    {
        var notebook = new Notebook<string>
                       {
                           { "a", Maybe.None<HalfSegment<int>>() },
                           { "b", Maybe.None<HalfSegment<int>>() }
                       };
        var segment = new Segment<int>(0, 0, 1);
        var halfSegment = new HalfSegment<int>(segment, true);

        var actual = notebook.Add("b", Maybe.Some(halfSegment));

        var index = actual.Match(_ => _, _ => -1);
        Assert.That(index, Is.EqualTo(1));
    }
}