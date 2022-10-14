using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Bot.Bot;

internal interface IBot
{
    Task<Maybe<string>> Process(string input, CancellationToken token);
}