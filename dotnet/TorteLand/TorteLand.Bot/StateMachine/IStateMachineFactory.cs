namespace TorteLand.Bot.StateMachine;

internal interface IStateMachineFactory
{
    IStateMachine Create();
}