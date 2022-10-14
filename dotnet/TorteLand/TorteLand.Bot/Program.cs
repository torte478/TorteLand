using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TorteLand.Bot;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices(
                   (context, services) =>
                   {
                       services.AddSingleton<ITelegramBotClient>(
                           _ =>
                           {
                               var token = context.Configuration.GetSection("Bot")["Token"];
                               Console.WriteLine(token);
                               return new TelegramBotClient(
                                   token: token);
                           });
                       services.AddSingleton<IBot, Bot>();
                       services.AddHostedService<Worker>();
                   })
               .Build();

await host.RunAsync();