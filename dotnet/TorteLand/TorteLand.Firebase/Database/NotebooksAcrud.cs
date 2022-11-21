using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Firebase.Integration;

namespace TorteLand.Firebase.Database;

// TODO : optimize
internal sealed class NotebooksAcrud : INotebooksAcrud
{
    private readonly AsyncLazy<IEntityAcrud> _acrud;

    public NotebooksAcrud(IEntityAcrudFactory factory)
    {
        _acrud = new AsyncLazy<IEntityAcrud>(factory.Create);
    }

    public async Task<Page<Unique<string>>> All(Maybe<Pagination> pagination, CancellationToken token)
    {
        var acrud = await _acrud;
        var entities = await acrud.All();

        return entities
               .Select((x, i) => new Unique<string>(i, x.Name))
               .Paginate(pagination, entities.Count);
    }

    public async Task<int> Create(string name, CancellationToken token)
    {
        var acrud = await _acrud;
        var id = await acrud.Create(name);
        var entities = await acrud.All();

        return entities
               .Select((x, i) => (x, i))
               .First(_ => _.x.Id == id)
               .i;
    }

    public async Task<Maybe<string>> Read(int index, CancellationToken token)
    {
        var acrud = await _acrud;
        var entities = await acrud.All();
        
        return index >= 0 && index < entities.Count
               ? entities.ElementAt(index).Name._(Maybe.Some)
               : Maybe.None<string>();
    }

    public async Task Delete(int index, CancellationToken token)
    {
        var acrud = await _acrud;
        var entities = await acrud.All();
        var id = entities.ElementAt(index).Id;
        await acrud.Delete(id);
    }

    public async Task Update(int index, string name, CancellationToken token)
    {
        var acrud = await _acrud;
        var entities = await acrud.All();
        var key = entities.ElementAt(index).Id;

        var notebook = await acrud.Read(key);
        var renamed = notebook with { Name = name };
        await acrud.Update(key, renamed);
    }
}