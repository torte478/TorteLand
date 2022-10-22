namespace TorteLand.Firebase.Database;

internal record TempNotebook(
    string Name,
    TempNote[] Notes);

internal record TempNote(
    string Text,
    int Weight);