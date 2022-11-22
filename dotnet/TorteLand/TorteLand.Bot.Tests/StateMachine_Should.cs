using FakeItEasy;
using NUnit.Framework;
using TorteLand.App.Client;
using TorteLand.Bot.Integration;
using TorteLand.Bot.StateMachine;
using TorteLand.Bot.StateMachine.States;
using TorteLand.Bot.Utils;

// ReSharper disable InconsistentNaming

namespace TorteLand.Bot.Tests;

[TestFixture]
internal sealed class StateMachine_Should
{
    [Test]
    public async Task ShowNoteName_WhenConfirmsDeleting()
    {
        var (machine, command) = Create("expected");
        
        var actual = await machine.Process(command, default);
        
        Assert.That(actual, Is.EqualTo("Delete 'expected'?"));
    }

    [Test]
    public async Task ReturnToPreviousState_OnConfirmCancel()
    {
        var confirm = A.Fake<ICommand>();
        A.CallTo(() => confirm.ToWord())
         .Returns(("n", confirm));
        
        var (machine, delete) = Create(string.Empty);
        
        await machine.Process(delete, default);
        await machine.Process(confirm, default);
        
        Assert.That(machine.ToString(), Is.EqualTo("StateMachine[NotebooksState]"));
    }

    private static (IStateMachine, ICommand) Create(string deleted)
    {
        var command = A.Fake<ICommand>();
        A.CallTo(() => command.ToName())
         .Returns(("remove", command));

        var note = new StringMaybe { IsSome = true, Value = deleted };
        var page = new StringUniquePage
                   {
                       Items = Array.Empty<StringUnique>()
                   };

        var client = A.Fake<INotebooksAcrudClient>();
        A.CallTo(() => client.ReadAsync(A<int?>._, A<CancellationToken>._))
         .Returns(Task.FromResult(note));
        A.CallTo(() => client.AllAsync(A<int?>._, A<int?>._, A<CancellationToken>._))
         .Returns(Task.FromResult(page));

        var factory = A.Fake<IClientFactory>();
        A.CallTo(() => factory.CreateNotebooksAcrudClient())
         .Returns(client);
        
        var machine = new StateMachine.StateMachine(10, factory, A.Fake<IRandom>());
        machine.SetState(new NotebooksState(10, client, machine));
        return (machine, command);
    }
}