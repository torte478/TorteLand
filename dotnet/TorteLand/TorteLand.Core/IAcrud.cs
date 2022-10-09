using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TorteLand.Core;

public interface IAcrud<TKey, TValue>
{
    IAsyncEnumerable<(TKey Key, TValue Value)> All(CancellationToken token);
    Task<(TKey Key, TValue Value)> Create(TValue value, CancellationToken token);
    Task<TValue> Read(TKey key, CancellationToken token);
    Task<TValue> Update(TKey key, TValue value, CancellationToken token);
    Task<TValue> Delete(TKey key, CancellationToken token);
}