namespace TorteLand.Firebase.Integration;

internal interface IEntityAcrudFactory
{
    Task<IEntityAcrud> Create();
}