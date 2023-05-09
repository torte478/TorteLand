using System;
using System.Text.Json.Serialization;

namespace TorteLand.Firebase.Integration.Tokens;

internal sealed class ExpiredToken
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; init; } = null!;

    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; init; } = null!;

    public TimeSpan Expired()
        => ExpiresIn
           ._(double.Parse)
           ._(TimeSpan.FromSeconds);
}