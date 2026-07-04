using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Smart_Learning_App.Services.IService;

namespace Smart_Learning_App.Services.MainService
{
    public class CacheService : ICacheServicecs
    {
        private readonly IDistributedCache cache;
        public CacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }
        public async Task<T> GetOrSetCaheAsync<T>(string CacheKey,
            Func<Task<T>> getDataAsync,
            TimeSpan? slidingExpiration = null,
            TimeSpan? absoluteExpireTime = null)
        {
            var CacheData = await cache.GetStringAsync(CacheKey);
            if (CacheData != null) { return JsonConvert.DeserializeObject<T>(CacheData)!; }
            var Data = await getDataAsync();
            var Options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(5),
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromHours(1),
            };
            var Result = JsonConvert.SerializeObject(Data);
            await cache.SetStringAsync(CacheKey , Result , Options);
            return Data;
        }
    }
}
