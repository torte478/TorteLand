using Telegram.Bot;
using TorteLand.App.Client;

namespace TorteLand.Bot.Integration;

internal interface IClientFactory
{
    INotebooksAcrudClient CreateNotebooksAcrudClient();
    INotebooksClient CreateNotebooksClient();
    ITelegramBotClient CreateTelegramBotClient();
}