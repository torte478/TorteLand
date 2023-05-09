using System.Threading.Tasks;
using TorteLand.Extensions;

namespace TorteLand.Bot.StateMachine.States;

internal static class CommandExtensions
{
    public static Task<string> ToUnknown(this string command)
        => $"Unknown command: {command}"
            ._(Task.FromResult<string>);
}