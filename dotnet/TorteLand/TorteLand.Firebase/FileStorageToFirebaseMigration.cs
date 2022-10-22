namespace TorteLand.Firebase;

// TODO : remove after using
public sealed class FileStorageToFirebaseMigration
{
    private readonly string _path;

    public FileStorageToFirebaseMigration(string path)
    {
        _path = path;
    }

    public Task Run(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}