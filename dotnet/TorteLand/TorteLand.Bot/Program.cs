// See https://aka.ms/new-console-template for more information

using System;
using System.Net.Http;
using TorteLand.App.Client;

using var httpClient = new HttpClient();
var client = new NotebooksAcrudClient("https://localhost:7083", httpClient);
var texts = await client.AllAsync();
foreach (var t in texts)
    Console.WriteLine($"{t.Id} {t.Value}");

