using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Firebase.Database;

internal sealed class Notebooks : INotebooks
{
    public Task<Page<Unique<Note>>> Read(int index, Maybe<Pagination> pagination, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, IReadOnlyCollection<string> values, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Either<IReadOnlyCollection<int>, Question>> Add(int index, Guid id, bool isRight, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task Rename(int index, int id, string text, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int index, int key, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}