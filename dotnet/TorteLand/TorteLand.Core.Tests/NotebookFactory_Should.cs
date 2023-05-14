using Microsoft.Extensions.Options;
using NUnit.Framework;
using SoftwareCraft.Functional;
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
        var settings = Options.Create(new NotebookSettings());
        var factory = new NotebookFactory(settings);

        var notebook = factory.Create(
            new[]
            {
                new Note("a", 5, Maybe.None<byte>()),
                new Note("b", 10, Maybe.None<byte>()),
                new Note("c", 1, Maybe.None<byte>()),
            });

        var actual = notebook.Select(_ => _.Value.Text);
        Assert.That(actual.SequenceEqual(new[] { "b", "a", "c"}), Is.True);
    }
}