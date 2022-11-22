using System;
using System.Net.Http;
using Telegram.Bot;
using TorteLand.App.Client;

// TODO : rename namespace
namespace TorteLand.Bot.Logic;

internal sealed class ClientFactory : IClientFactory, IDisposable
{
    private readonly string _url;
    private readonly string _token;
    private readonly HttpClient _client;

    public ClientFactory(string url, string token, IHttpClientFactory factory)
    {
        _url = url;
        _token = token;
        _client = factory.CreateClient();
    }

    // TODO : to singleton
    public INotebooksAcrudClient CreateNotebooksAcrudClient()
        => new NotebooksAcrudClient(_url, _client);

    public INotebooksClient CreateNotebooksClient()
        => new NotebooksClient(_url, _client);

    public ITelegramBotClient CreateTelegramBotClient()
        => new TelegramBotClient(_token, _client);

    public void Dispose()
    {
        _client.Dispose();
    }
}