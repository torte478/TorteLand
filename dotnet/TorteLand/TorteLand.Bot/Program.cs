using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using TorteLand.Bot.Logic;
using TorteLand.Bot.StateMachine;
using TorteLand.Bot.Utils;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices(
                   (context, services) =>
                   {
                       services.AddSingleton<IClock, Clock>();
                       services.AddSingleton<IRandom, Random>();

                       services.AddHttpClient()
                               .RemoveAll<IHttpMessageHandlerBuilderFilter>();

                       services.AddSingleton<IClientFactory>(
                           _ => new ClientFactory(
                               context.Configuration.GetSection("TorteLand.App")["Url"],
                               context.Configuration.GetSection("Bot")["Token"],
                               _.GetRequiredService<IHttpClientFactory>()));

                       services.AddSingleton<ICommandFactory, CommandFactory>();
                       services.AddSingleton<IStateMachineFactory, StateMachineFactory>();
                       services.AddHostedService<Worker>();
                   })
               .Build();

await host.RunAsync();