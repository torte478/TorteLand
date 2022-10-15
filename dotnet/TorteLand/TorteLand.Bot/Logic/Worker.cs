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
using TorteLand.Bot.StateMachine;
using TorteLand.Bot.Utils;

namespace TorteLand.Bot.Logic;

internal sealed class Worker : BackgroundService
{
    private readonly DateTime _start;

    private readonly ITelegramBotClient _client;
    private readonly ICommandFactory _commands;
    private readonly IStateMachine _logic;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IClientFactory clientFactory,
        ICommandFactory commands,
        IStateMachineFactory stateMachineFactory,
        IClock clock,
        ILogger<Worker> logger)
    {
        _start = clock.Now();
        _client = clientFactory.CreateTelegramBotClient();
        _commands = commands;
        _logic = stateMachineFactory.Create();
        _logger = logger;
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

        var result = await ProcessCommand(text, token);

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: result,
            cancellationToken: token);
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

    private async Task<string> ProcessCommand(string text, CancellationToken token)
    {
        try
        {
            var command = _commands.Create(text);
            var response = await _logic.Process(command, token);
            return response is { Length: > 0 } ? response : "{}";
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.ToString());
            return ex.Message;
        }
    }
}