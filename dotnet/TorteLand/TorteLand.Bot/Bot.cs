using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.Bot;

internal sealed class Bot : IBot
{
    public Task<Maybe<string>> Process(string input, CancellationToken token)
    {
        return input._(Maybe.Some)._(Task.FromResult);
    }
}