using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class ContractApiIntegrationTests
{
    private readonly HttpClient _client;

    public ContractApiIntegrationTests()
    {
        _client = new HttpClient();

        _client.BaseAddress =
            new Uri("https://localhost:7200/");
    }

    [Fact]
    public async Task GetContracts_Returns200()
    {
        var response =
            await _client.GetAsync("api/contracts");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetContracts_ReturnsJson()
    {
        var response =
            await _client.GetStringAsync("api/contracts");

        Assert.NotNull(response);
    }
}