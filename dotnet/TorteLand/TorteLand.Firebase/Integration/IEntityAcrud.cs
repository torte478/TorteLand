namespace TorteLand.Firebase.Integration;

internal interface IEntityAcrud
{
    Task<IReadOnlyCollection<(string Id, string Name)>> All();
    Task<string> Create(string name);
    Task<(string Id, string Name)> Read(int index);
    Task<NotebookEntity> Read(string id);
    Task Update(string id, NotebookEntity entity);
    Task Delete(string id);
}