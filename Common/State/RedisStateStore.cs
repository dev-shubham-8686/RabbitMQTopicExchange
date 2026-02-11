using StackExchange.Redis;
using System.Text.Json;

namespace Common.State
{

    public sealed class RedisStateStore : IDisposable
    {
        private readonly ConnectionMultiplexer _mux;
        private readonly IDatabase _db;
        private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RedisStateStore(RedisOptions opt)
        {
            _mux = ConnectionMultiplexer.Connect(opt.ConnectionString);
            _db = _mux.GetDatabase();
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null) =>
            expiry == null
                ? _db.StringSetAsync(key, JsonSerializer.Serialize(value, _json))
                : _db.StringSetAsync(key, JsonSerializer.Serialize(value, _json), new StackExchange.Redis.Expiration(expiry.Value));

        public async Task<T?> GetAsync<T>(string key)
        {
            var v = await _db.StringGetAsync(key);
            return v.HasValue ? JsonSerializer.Deserialize<T>(v!, _json) : default;
        }

        public Task<bool> DeleteAsync(string key) => _db.KeyDeleteAsync(key);

        public void Dispose() => _mux.Dispose();
    }

}
