namespace Smart_Learning_App.Services.IService
{
    public interface ICacheServicecs 
    {
        Task<T> GetOrSetCaheAsync<T>
            (
             string CacheKey ,
             Func<Task<T>>getDataAsync ,
             TimeSpan ? slidingExpiration = null ,
             TimeSpan ? absoluteExpireTime = null
            );
    }
}
