using SoftwareCraft.Functional;

namespace TorteLand.App.Extensions;

internal static class PlusesExtensions
{
    public static byte ToByte(this Maybe<byte> pluses)
        => pluses.Match(
            _ => _,
            () => default);
}