using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Tests.Integration.HealthChecks;

public class HealthCheckEndpointsTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public HealthCheckEndpointsTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Endpoint /health/live deve retornar 200 OK com status Healthy")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthLive_DeveRetornar200ComStatusHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "o endpoint de liveness deve retornar 200 OK");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var json = JsonDocument.Parse(content);
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy", "o status geral deve ser Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/live deve conter check 'self' na resposta JSON")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthLive_DeveConterCheckSelfNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        var checks = json.RootElement.GetProperty("checks").EnumerateArray().ToList();
        checks.Should().HaveCount(1, "o endpoint /health/live deve retornar apenas o check com tag 'live'");

        var selfCheck = checks.FirstOrDefault(c => c.GetProperty("name").GetString() == "self");
        selfCheck.ValueKind.Should().NotBe(JsonValueKind.Undefined, "deve existir um check chamado 'self'");
        selfCheck.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/ready deve retornar 200 OK com status Healthy")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthReady_DeveRetornar200ComStatusHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "o endpoint de readiness deve retornar 200 OK");

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy", "o status geral deve ser Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/ready deve conter check 'database' na resposta JSON")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthReady_DeveConterCheckDatabaseNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        var checks = json.RootElement.GetProperty("checks").EnumerateArray().ToList();
        checks.Should().HaveCount(1, "o endpoint /health/ready deve retornar apenas checks com tag 'ready'");

        var databaseCheck = checks.FirstOrDefault(c => c.GetProperty("name").GetString() == "database");
        databaseCheck.ValueKind.Should().NotBe(JsonValueKind.Undefined, "deve existir um check chamado 'database'");
        databaseCheck.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/startup deve retornar 200 OK com status Healthy")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthStartup_DeveRetornar200ComStatusHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/startup");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "o endpoint de startup deve retornar 200 OK");

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy", "o status geral deve ser Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/startup deve conter check 'database' na resposta JSON")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthStartup_DeveConterCheckDatabaseNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/startup");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        var checks = json.RootElement.GetProperty("checks").EnumerateArray().ToList();
        checks.Should().HaveCount(1, "o endpoint /health/startup deve usar o mesmo filtro do /health/ready");

        var databaseCheck = checks.FirstOrDefault(c => c.GetProperty("name").GetString() == "database");
        databaseCheck.ValueKind.Should().NotBe(JsonValueKind.Undefined, "deve existir um check chamado 'database'");
    }

    [Fact(DisplayName = "Resposta JSON dos endpoints deve conter totalDuration")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_DeveConterTotalDurationNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("totalDuration", out var totalDuration).Should().BeTrue("a resposta deve conter a propriedade 'totalDuration'");
        totalDuration.ValueKind.Should().Be(JsonValueKind.String, "totalDuration deve ser uma string representando TimeSpan");
    }

    [Fact(DisplayName = "Resposta JSON dos checks deve conter name, status, description e duration")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_ChecksDevemConterPropriedadesObrigatorias()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        var checks = json.RootElement.GetProperty("checks").EnumerateArray().ToList();
        var check = checks.First();

        check.TryGetProperty("name", out _).Should().BeTrue("cada check deve conter a propriedade 'name'");
        check.TryGetProperty("status", out _).Should().BeTrue("cada check deve conter a propriedade 'status'");
        check.TryGetProperty("description", out _).Should().BeTrue("cada check deve conter a propriedade 'description'");
        check.TryGetProperty("duration", out _).Should().BeTrue("cada check deve conter a propriedade 'duration'");
    }

    [Fact(DisplayName = "Resposta deve ter Content-Type application/json")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_DeveRetornarContentTypeJson()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json", "o response writer deve definir Content-Type como application/json");
    }

    [Fact(DisplayName = "Resposta JSON deve estar formatada (indented)")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_RespostaDeveEstarFormatada()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\n", "a resposta JSON deve estar indentada (WriteIndented = true)");
    }
}
