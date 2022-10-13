namespace TorteLand.Core.Notebooks;

internal interface IFactory
{
    ITransactionNotebook Create();
}