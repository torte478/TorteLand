namespace TorteLand.Core.Notebooks;

internal interface IKeyGenerator<out TKey>
{
    TKey Next();
}

internal sealed class IntKeyGenerator : IKeyGenerator<int>
{
    private int _next;

    public int Next() => _next++;
}