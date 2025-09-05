using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Concurrent;

namespace Web.DataAccess.Tests;

internal class FakeHybridCache : HybridCache
{
    private readonly ConcurrentDictionary<string, object?> _cache = new();

    public override async ValueTask<T> GetOrCreateAsync<TState, T>(
        string key,
        TState state,
        Func<TState, CancellationToken, ValueTask<T>> factory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var existing) && existing is T cachedValue)
        {
            return cachedValue;
        }

        var value = await factory(state, cancellationToken);
        _cache[key] = value;
        return value;
    }

    public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return ValueTask.CompletedTask;
    }

    public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public override ValueTask SetAsync<T>(
        string key,
        T value,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        _cache[key] = value;
        return ValueTask.CompletedTask;
    }
}
