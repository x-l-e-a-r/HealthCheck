using System.Net;
using System.Text.Json;
using FluentAssertions;
using HealthCheck.Extensions.Common.BaseHttpService;
using RichardSzalay.MockHttp;

namespace HealthCheck.Extensions.Common.Tests.BaseHttpServiceTests.BaseHttpClientTests;

public class GetAsyncShould
{
    public GetAsyncShould()
    {
    }

    [Fact]
    public async Task ReturnDeserializedResponse()
    {
        const string mockUrl = "http://localhost/";
        var response = new SampleModel()
        {
            Id = "someId"
        };
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, response);
        
        var mockHttpHandler = new MockHttpMessageHandler();
        mockHttpHandler
            .When(mockUrl)
            .Respond("application/json", stream);
        
        var httpClient = new HttpClient(mockHttpHandler);

        var sut = new BaseHttpClient(httpClient);

        var result = await sut.GetAsync<SampleModel>(mockUrl);
        result.Should().BeOfType<SampleModel>();
        result.Id.Should().Be("someId");
    }

    [Fact]
    public async Task ThrowHttpRequestException_Given_InvalidJson()
    {
        const string mockUrl = "http://localhost/";
        var invalidJsonResponse = "}{";
        var mockHttpHandler = new MockHttpMessageHandler();
        mockHttpHandler
            .When(mockUrl)
            .Respond("application/json", invalidJsonResponse);
        
        var httpClient = new HttpClient(mockHttpHandler);

        var sut = new BaseHttpClient(httpClient);

        var action = async () => await sut.GetAsync<SampleModel>(mockUrl);
        await action.Should().ThrowAsync<JsonException>().WithMessage("Failed to deserialize*");
    }

    [Fact]
    public async Task ThrowHttpRequestException_Given_FailureResponse()
    {
        const string mockUrl = "http://localhost/";
        var invalidJsonResponse = "}{";
        var mockHttpHandler = new MockHttpMessageHandler();
        mockHttpHandler
            .When(mockUrl)
            .Respond(HttpStatusCode.BadGateway);
        
        var httpClient = new HttpClient(mockHttpHandler);

        var sut = new BaseHttpClient(httpClient);

        var action = async () => await sut.GetAsync<SampleModel>(mockUrl);
        await action.Should().ThrowAsync<HttpRequestException>().WithMessage("HttpRequest failed*");
    }
    
    
    private class SampleModel
    {
        public string Id { get; set; }
    }
}