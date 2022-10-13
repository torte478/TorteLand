namespace TorteLand.Core.Notebooks;

internal interface IFactory
{
    IAsyncNotebook Create();
}