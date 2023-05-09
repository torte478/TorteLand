using System;
using Firebase.Database;
using Microsoft.Extensions.Options;
using TorteLand.Firebase.Integration.Tokens;

namespace TorteLand.Firebase.Integration;

internal sealed class FirebaseClientFactory : IFirebaseClientFactory, IDisposable
{
    private readonly IToken _token;
    private readonly FirebaseSettings _settings;

    private FirebaseClient? _client;

    public FirebaseClientFactory(IToken token, IOptions<FirebaseSettings> settings)
    {
        _token = token;
        _settings = settings.Value;
    }

    public FirebaseClient Create()
        => _client ??= CreateClient();

    private FirebaseClient CreateClient()
    {
        return new FirebaseClient(
            baseUrl: _settings.Url,
            options: new FirebaseOptions
                     {
                         AuthTokenAsyncFactory = _token.Provide
                     });
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}