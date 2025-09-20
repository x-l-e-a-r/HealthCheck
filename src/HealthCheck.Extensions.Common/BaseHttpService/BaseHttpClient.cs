using System.Text.Json;

namespace HealthCheck.Extensions.Common.BaseHttpService;

public class BaseHttpClient : IBaseHttpClient
{
    private readonly HttpClient _httpClient;
    
    public BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public BaseHttpClient(HttpMessageHandler handler)
    {
        _httpClient = new HttpClient(handler);
    }

    public async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"HttpRequest failed with StatusCode: {response.StatusCode} - {response.ReasonPhrase}");

        var streamContent = await response.Content.ReadAsStreamAsync();
        T? deserializedResult;
        try
        {
            deserializedResult = await JsonSerializer.DeserializeAsync<T>(streamContent);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to deserialize response.", ex);
        }
        
        return deserializedResult!;
    }

    public T PostAsync<T>()
    {
        throw new NotImplementedException();
    }

    public T PutAsync<T>()
    {
        throw new NotImplementedException();
    }

    public T DeleteAsync<T>()
    {
        throw new NotImplementedException();
    }
}