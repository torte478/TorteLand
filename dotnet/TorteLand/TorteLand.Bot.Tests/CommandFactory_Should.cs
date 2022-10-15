using NUnit.Framework;
using TorteLand.Bot.Logic;

// ReSharper disable InconsistentNaming

namespace TorteLand.Bot.Tests;

[TestFixture]
internal sealed class CommandFactory_Should
{
    private ICommandFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new CommandFactory();
    }

    [Test]
    public void SplitByNewLine_OnCommandCreate()
    {
        var command = _factory.Create("add\nfirst\nsecond");

        Assert.That(command.GetLine(), Is.EqualTo("first"));
    }

    [Test]
    public void KeepNewLines_OnCommandCreate()
    {
        var command = _factory.Create("add\n1 2\n3");

        Assert.That(command.GetLine(1), Is.EqualTo("3"));
    }

    [Test]
    public void SplitBySpace_ForFirstLine()
    {
        var command = _factory.Create("open 1");

        Assert.That(command.GetInt(), Is.EqualTo(1));
    }

    [Test]
    public void CreateCommand_OnSingleWord()
    {
        var command = _factory.Create("all");

        Assert.That(command.Name, Is.EqualTo("all"));
    }

    [Test]
    public void CreateDifferentArguments_OnSingleLine()
    {
        var command = _factory.Create("rename 1 text");

        Assert.That(command.GetLine(1), Is.EqualTo("text"));
    }
}