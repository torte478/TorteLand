using NUnit.Framework;
using TorteLand.Bot.Logic;

// ReSharper disable InconsistentNaming

namespace TorteLand.Bot.Tests;

[TestFixture]
internal sealed class Command_Should
{
    [Test]
    public void ReturnFirstInt_OnDefaultIndex()
    {
        var command = new Command("name", new[] { "1", "second" });

        var actual = command.GetInt(0);

        Assert.That(actual, Is.EqualTo(1));
    }

    [Test]
    public void ReturnFirstString_OnDefaultIndex()
    {
        var command = new Command("name", new[] { "first", "2" });

        var actual = command.GetLine(0);

        Assert.That(actual, Is.EqualTo("first"));
    }

    [Test]
    public void ReturnLines_OnGetLines()
    {
        var command = new Command("name", new[] { "1", "2" });

        var actual = command.GetLines(0);

        Assert.That(actual, Is.EquivalentTo(new[] { "1", "2" }));
    }
}