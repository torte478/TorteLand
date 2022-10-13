using System.Reflection;
using TorteLand.Core.Contracts;

namespace TorteLand.FileStorage.Storages;

internal sealed class Storage : IStorage
{
    private readonly string _path;
    private readonly ITransactionFactory _factory;

    public Storage(string file, ITransactionFactory factory)
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var full = Path.Combine(location, file);
        _path = full;

        _factory = factory;
    }

    public ITransaction StartTransaction()
        => _factory.Create(_path);
}