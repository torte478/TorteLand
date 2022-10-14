﻿using System.Reflection;
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

        Assert.That(command.GetString(), Is.EqualTo("first"));
    }

    [Test]
    public void KeepNewLines_OnCommandCreate()
    {
        var command = _factory.Create("add\n1 2\n3");

        Assert.That(command.GetString(1), Is.EqualTo("3"));
    }

    [Test]
    public void SplitBySpace_ForFirstLine()
    {
        var command = _factory.Create("open 1");

        Assert.That(command.GetInt(), Is.EqualTo(1));
    }
}