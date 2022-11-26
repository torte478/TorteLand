using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Firebase.Database;
using Microsoft.Extensions.Options;

namespace TorteLand.Firebase.Integration;

internal sealed class FirebaseClientFactory : IFirebaseClientFactory, IDisposable
{
    private const string Uri = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={0}";
    
    private readonly FirebaseSettings _settings;
    private readonly HttpClient _http;

    private FirebaseClient? _client;

    public FirebaseClientFactory(IOptions<FirebaseSettings> settings, IHttpClientFactory factory)
    {
        _settings = settings.Value;
        _http = factory.CreateClient();
    }

    public async Task<FirebaseClient> Create()
        => _client ??= await CreateClient();

    private async Task<FirebaseClient> CreateClient()
    {
        var content = new Dictionary<string, string>
                      {
                          { "email", _settings.Email },
                          { "password", _settings.Password },
                          { "returnSecureToken", "true" }
                      }
                      ._(_ => JsonSerializer.Serialize(_))
                      ._(_ => new StringContent(_));

        var response = await _http.PostAsync(
                           string.Format(Uri, _settings.ApiKey),
                           content);

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonNode.Parse(json)!["idToken"]!.ToString();

        return new FirebaseClient(
            baseUrl: _settings.Url,
            options: new FirebaseOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token)
                     });
    }

    public void Dispose()
    {
        _client?.Dispose();
        _http.Dispose();
    }
}