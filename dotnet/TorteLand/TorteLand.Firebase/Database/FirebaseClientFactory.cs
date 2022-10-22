using Firebase.Auth;
using Firebase.Database;

namespace TorteLand.Firebase.Database;

internal sealed class FirebaseClientFactory : IFirebaseClientFactory, IDisposable
{
    private readonly Credentials _credentials;

    private FirebaseClient? _client;

    public FirebaseClientFactory(Credentials credentials)
    {
        _credentials = credentials;
    }

    public async ValueTask<FirebaseClient> Create()
        => _client ??= await CreateClient();

    private async Task<FirebaseClient> CreateClient()
    {
        var auth = new FirebaseAuthProvider(new FirebaseConfig(_credentials.ApiKey));
        var token = await auth.SignInWithEmailAndPasswordAsync(_credentials.Email, _credentials.Password);
        return new FirebaseClient(
            baseUrl: _credentials.Url,
            options: new FirebaseOptions
                     {
                         AuthTokenAsyncFactory = () => token.FirebaseToken._(Task.FromResult)
                     });
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}