using System.Threading.Tasks;
using SoftwareCraft.Functional;

namespace TorteLand.App.Extensions;

internal static class EitherExtensions
{
    public static async Task<Models.Either<TLeft, TRight>> ToModel<TLeft, TRight>(this Task<Either<TLeft, TRight>> origin)
    {
        var result = await origin;

        return result.Match(
            _ => new Models.Either<TLeft, TRight>(_, default),
            _ => new Models.Either<TLeft, TRight>(default, _));
    }
}