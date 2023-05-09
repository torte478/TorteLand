using Microsoft.Extensions.DependencyInjection;
using TorteLand.Contracts;
using TorteLand.Utils;

namespace TorteLand;

public static class IocExtensions
{
    public static IServiceCollection AddTorteLand(this IServiceCollection services)
        => services
            .AddSingleton<IClock, Clock>();
}