namespace TorteLand.Firebase.Integration;

internal interface INotebookEntityAcrudFactory
{
    Task<INotebookEntityAcrud> Create();
}