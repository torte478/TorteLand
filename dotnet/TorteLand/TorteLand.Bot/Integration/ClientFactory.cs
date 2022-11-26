using System;
using System.Net.Http;
using Telegram.Bot;
using TorteLand.App.Client;

namespace TorteLand.Bot.Integration;

internal sealed class ClientFactory : IClientFactory, IDisposable
{
    private readonly Lazy<HttpClient> _httpClient;
    private readonly Lazy<INotebooksAcrudClient> _notebooksAcrudClient;
    private readonly Lazy<INotebooksClient> _notebooksClient;
    private readonly Lazy<ITelegramBotClient> _telegramBotClient;

    public ClientFactory(string url, string token, IHttpClientFactory factory)
    {
        _httpClient = new Lazy<HttpClient>(factory.CreateClient);
        _notebooksAcrudClient = new Lazy<INotebooksAcrudClient>(() => new NotebooksAcrudClient(url, _httpClient.Value));
        _notebooksClient = new Lazy<INotebooksClient>(() => new NotebooksClient(url, _httpClient.Value));
        _telegramBotClient = new Lazy<ITelegramBotClient>(() => new TelegramBotClient(token, _httpClient.Value));
    }

    public INotebooksAcrudClient CreateNotebooksAcrudClient() 
        => _notebooksAcrudClient.Value;

    public INotebooksClient CreateNotebooksClient()
        => _notebooksClient.Value;

    public ITelegramBotClient CreateTelegramBotClient()
        => _telegramBotClient.Value;

    public void Dispose()
    {
        if (_httpClient.IsValueCreated)
            _httpClient.Value.Dispose();
    }
}