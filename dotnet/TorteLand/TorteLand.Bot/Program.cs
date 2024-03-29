using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using TorteLand;
using TorteLand.Bot.Integration;
using TorteLand.Bot.StateMachine;
using TorteLand.Bot.Utils;
using TorteLand.Extensions;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices(
                   (context, services) =>
                   {
                       services.AddTorteLand();
                       services.AddSingleton<IRandom, Random>();

                       services.AddHttpClient()
                               .RemoveAll<IHttpMessageHandlerBuilderFilter>();

                       // TODO: settings to normal
                       services.AddSingleton<IClientFactory>(
                           _ => new ClientFactory(
                               context.Configuration.GetSection("TorteLand.App")["Url"],
                               context.Configuration.GetSection("Bot")["Token"],
                               _.GetRequiredService<IHttpClientFactory>()));

                       services.AddSingleton<ICommandFactory, CommandFactory>();

                       services.AddSingleton<IStateMachineFactory>(
                           _ => new StateMachineFactory(
                               pageSize: context.Configuration.GetSection("Pagination")["PageSize"]._(int.Parse),
                               factory: _.GetRequiredService<IClientFactory>(),
                               random: _.GetRequiredService<IRandom>()));

                       services.AddHostedService<Worker>();
                   })
               .Build();

await host.RunAsync();