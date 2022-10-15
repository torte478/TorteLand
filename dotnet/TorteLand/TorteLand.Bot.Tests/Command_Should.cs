using NUnit.Framework;
using TorteLand.Bot.Logic;

// ReSharper disable InconsistentNaming

namespace TorteLand.Bot.Tests;

[TestFixture]
internal sealed class Command_Should
{
    [Test]
    public void ReturnInt_AfterName()
    {
        var command = new Command("name 1");

        var (_, arguments) = command.ToName();
        var (actual, _) = arguments.ToInt();

        Assert.That(actual, Is.EqualTo(1));
    }

    [Test]
    [TestCase("name a", "a")]
    [TestCase("name b c", "b c")]
    public void ReturnString_AfterName(string raw, string expected)
    {
        var command = new Command(raw);

        var (_, arguments) = command.ToName();
        var (actual, _) = arguments.ToLine();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ReturnLines_AfterName()
    {
        var command = new Command($"name a{Environment.NewLine}b");

        var (_, arguments) = command.ToName();
        var actual = arguments.ToLines();

        Assert.That(actual, Is.EquivalentTo(new[] { "a", "b" }));
    }

    [Test]
    [TestCase("all", "all")]
    [TestCase("create test", "create")]
    [TestCase("add\r\nfirst\r\nsecond", "add")]
    [TestCase("add\nfirst\nsecond", "add")]
    public void ReturnName_BeforeArguments(string raw, string expected)
    {
        var command = new Command(raw);

        var (actual, _) = command.ToName();

        Assert.That(actual, Is.EqualTo(expected));
    }
}