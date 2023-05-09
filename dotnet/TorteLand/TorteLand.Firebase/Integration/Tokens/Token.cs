using System;
using System.Threading.Tasks;
using TorteLand.Contracts;

namespace TorteLand.Firebase.Integration.Tokens;

internal sealed class Token : IToken
{
    private static readonly TimeSpan Offset = TimeSpan.FromSeconds(10);
    
    private readonly IExpiredToken _origin;
    private readonly IClock _clock;
    
    private string _token = string.Empty;
    private DateTimeOffset _expired = DateTimeOffset.MinValue;
    
    public Token(IExpiredToken origin, IClock clock)
    {
        _origin = origin;
        _clock = clock;
    }

    public async Task<string> Provide()
    {
        var now = _clock.ToNow();
        if (now > _expired)
            await ReinitToken(now);

        return _token;
    }

    private async Task ReinitToken(DateTimeOffset now)
    {
        var token = await _origin.Provide();
        _token = token.IdToken;
        _expired = now.Add(token.Expired()).Subtract(Offset);
    }
}