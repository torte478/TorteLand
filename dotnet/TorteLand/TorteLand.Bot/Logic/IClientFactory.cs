using Telegram.Bot;
using TorteLand.App.Client;

namespace TorteLand.Bot.Logic;

internal interface IClientFactory
{
    INotebooksAcrudClient CreateNotebooksAcrudClient();
    INotebooksClient CreateNotebooksClient();
    ITelegramBotClient CreateTelegramBotClient();
}