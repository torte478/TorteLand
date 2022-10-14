using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TorteLand.Bot.Bot;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot;

internal sealed class Worker : BackgroundService
{
    private readonly DateTime _start;

    private readonly ITelegramBotClient _client;
    private readonly IBot _bot;
    private readonly ILogger<Worker> _logger;

    public Worker(IClientFactory factory, IBot bot, IClock clock, ILogger<Worker> logger)
    {
        _client = factory.CreateTelegramBotClient();
        _bot = bot;
        _logger = logger;

        _start = clock.Now();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.StartReceiving(
            updateHandler: OnUpdate,
            pollingErrorHandler: OnError,
            receiverOptions: new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } },
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task OnUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Message is not { Text: { } text } message)
            return;

        if (message.Date < _start)
            return;

        Console.WriteLine(update.Message.Date);

        var response = await _bot.Process(text, token);

        await response.MatchAsync(
            _ => client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _,
                cancellationToken: token),
            () => Task.CompletedTask);
    }

    private Task OnError(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var message = exception switch
        {
            ApiRequestException apiException
                => string.Format(
                    "Telegram API Error: [{0}] {1}",
                    apiException.ErrorCode,
                    apiException.Message),
            _ => exception.Message
        };

        _logger.Log(LogLevel.Error, message);
        return Task.CompletedTask;
    }
}