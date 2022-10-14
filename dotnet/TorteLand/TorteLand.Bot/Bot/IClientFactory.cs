using Telegram.Bot;
using TorteLand.App.Client;

namespace TorteLand.Bot.Bot;

internal interface IClientFactory
{
    INotebooksAcrudClient CreateNotebooksAcrudClient();
    INotebooksClient CreateNotebooksClient();
    ITelegramBotClient CreateTelegramBotClient();
}