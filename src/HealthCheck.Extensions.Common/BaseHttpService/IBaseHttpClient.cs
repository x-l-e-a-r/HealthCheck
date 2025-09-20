namespace HealthCheck.Extensions.Common.BaseHttpService;

public interface IBaseHttpClient
{
    Task<T> GetAsync<T>(string url);
    T PostAsync<T>();
    T PutAsync<T>();
    T DeleteAsync<T>();
}