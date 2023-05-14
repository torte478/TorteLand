namespace TorteLand.App.Models;

public sealed record Note(
    int Id,
    string Text,
    byte Pluses);