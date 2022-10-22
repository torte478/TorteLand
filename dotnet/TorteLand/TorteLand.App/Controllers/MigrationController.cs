using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TorteLand.Firebase;

namespace TorteLand.App.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class MigrationController : ControllerBase
{
    private readonly FileStorageToFirebaseMigration _migration;

    public MigrationController(FileStorageToFirebaseMigration migration)
    {
        _migration = migration;
    }

    [HttpPost]
    public Task Run(CancellationToken token)
        => _migration.Run(token);
}