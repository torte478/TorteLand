using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TorteLand.App.Models;
using TorteLand.Core;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ProjectController : ControllerBase
{
    private readonly IAcrud<int, string> _acrud;

    public ProjectController(IAcrud<int, string> acrud)
    {
        _acrud = acrud;
    }

    [HttpGet]
    public async IAsyncEnumerable<Project> Get([EnumeratorCancellation] CancellationToken token)
    {
        await foreach (var x in _acrud.All(token))
            yield return new Project(x.Key, x.Value);
    }

    [HttpPost]
    public Task Add(string name, CancellationToken token)
        => _acrud.Create(name, token);

    [HttpDelete]
    public Task Delete(int key, CancellationToken token)
        => _acrud.Delete(key, token);

    [HttpPut]
    public Task Update(int key, string name, CancellationToken token)
        => _acrud.Update(key, name, token);
}