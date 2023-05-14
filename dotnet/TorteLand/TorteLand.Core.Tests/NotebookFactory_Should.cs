using NUnit.Framework;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Core.Notebooks;

// ReSharper disable InconsistentNaming

namespace TorteLand.Core.Tests;

[TestFixture]
internal sealed class NotebookFactory_Should
{
    [Test]
    public void SaveOrder_AfterCreate()
    {
        var factory = new NotebookFactory();

        var notebook = factory.Create(
            new[]
            {
                new Note("a", 5),
                new Note("b", 10),
                new Note("c", 1),
            });

        var actual = notebook.Select(_ => _.Value.Text);
        Assert.That(actual.SequenceEqual(new[] { "b", "a", "c"}), Is.True);
    }
}