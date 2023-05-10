using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TorteLand.Extensions;

namespace TorteLand.Firebase.Integration.Tokens;

internal sealed class RemoteExpiredToken : IExpiredToken, IDisposable
{
    private const string Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={0}";
    
    private readonly FirebaseSettings _settings;
    private readonly HttpClient _http;

    public RemoteExpiredToken(IOptions<FirebaseSettings> settings, IHttpClientFactory factory)
    {
        _settings = settings.Value;
        _http = factory.CreateClient();
    }

    public async Task<ExpiredToken> Provide(CancellationToken cancellation)
    {
        var content = new Dictionary<string, string>
                      {
                          { "email", _settings.Email },
                          { "password", _settings.Password },
                          { "returnSecureToken", "true" }
                      }
                      ._(_ => JsonSerializer.Serialize(_))
                      ._<StringContent>();

        var response = await _http.PostAsync(
                           string.Format(Uri, _settings.ApiKey),
                           content,
                           cancellation);

        // TODO: to stream
        var json = await response.Content.ReadAsStringAsync(cancellation);
        var token = JsonSerializer.Deserialize<ExpiredToken>(json);
        
        return token!;
    }

    public void Dispose()
    {
        _http.Dispose();
    }
}