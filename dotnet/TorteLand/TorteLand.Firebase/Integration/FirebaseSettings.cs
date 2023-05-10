using System;

namespace TorteLand.Firebase.Integration;

internal sealed class FirebaseSettings
{
    public string Root { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public TimeSpan Timeout { get; set; }
}
    