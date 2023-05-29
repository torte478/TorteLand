using SoftwareCraft.Functional;

namespace TorteLand.Core.Contracts.Notebooks.Models;

public sealed record Note(
    string Text,
    int Weight,
    Maybe<byte> Pluses);