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
        var command = new Command("name", new[] { "name", "1", "second" });

        var actual = command.GetInt(0);

        Assert.That(actual, Is.EqualTo(1));
    }

    [Test]
    public void ReturnFirstString_OnDefaultIndex()
    {
        var command = new Command("name", new[] { "name", "first", "2" });

        var actual = command.GetString(0);

        Assert.That(actual, Is.EqualTo("first"));
    }

    [Test]
    public void ReturnLines_OnGetTail()
    {
        var command = new Command("name", new[] { "name", "1", "2" });

        var actual = command.GetTail(0);

        Assert.That(actual, Is.EqualTo($"1{Environment.NewLine}2"));
    }
}