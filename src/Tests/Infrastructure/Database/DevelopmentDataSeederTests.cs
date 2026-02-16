using API.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Tests.Infrastructure.Database;

[Trait("Infrastructure", "Database")]
public class DevelopmentDataSeederTests
{
    [Fact(DisplayName = "SeedIfDevelopment não deve lançar exceção quando ambiente não for Development")]
    public void SeedIfDevelopment_NaoDeveLancarExcecao_QuandoAmbienteNaoForDevelopment()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Production
        });
        
        using var app = builder.Build();

        // Act & Assert
        FluentActions.Invoking(() => DevelopmentDataSeeder.SeedIfDevelopment(app))
            .Should().NotThrow();
    }

    [Fact(DisplayName = "Seed não deve lançar exceção quando estiver em ambiente de testes")]
    public void Seed_NaoDeveLancarExcecao_QuandoEstiverEmAmbienteDeTestes()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        
        using var app = builder.Build();

        // Act & Assert
        FluentActions.Invoking(() => DevelopmentDataSeeder.Seed(app))
            .Should().NotThrow("porque IsIntegrationTest() deve retornar true e o método deve retornar cedo");
    }

    [Fact(DisplayName = "SeedIfDevelopment não deve executar seed quando ambiente não é Development")]
    [Trait("Infrastructure", "Database")]
    public void SeedIfDevelopment_NaoDeveExecutarSeed_QuandoAmbienteNaoDevelopment()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Staging
        });
        
        using var app = builder.Build();

        // Act
        var inicio = DateTime.UtcNow;
        DevelopmentDataSeeder.SeedIfDevelopment(app);
        var duracao = DateTime.UtcNow - inicio;

        // Assert
        duracao.Should().BeLessThan(TimeSpan.FromMilliseconds(100), "porque deve retornar imediatamente sem executar seed");
    }

    [Fact(DisplayName = "Seed deve retornar sem executar quando em integração")]
    [Trait("Infrastructure", "Database")]
    public void Seed_DeveRetornarSemExecutar_QuandoEmIntegracao()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        
        using var app = builder.Build();

        // Act
        var inicio = DateTime.UtcNow;
        DevelopmentDataSeeder.Seed(app);
        var duracao = DateTime.UtcNow - inicio;

        // Assert
        duracao.Should().BeLessThan(TimeSpan.FromMilliseconds(100), "porque IsIntegrationTest retorna true e deve retornar cedo");
    }

    [Fact(DisplayName = "Seed deve lançar InvalidOperationException quando falhar ao semear")]
    [Trait("Infrastructure", "Database")]
    public void Seed_DeveLancarInvalidOperationException_QuandoFalharAoSemear()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        using var app = builder.Build();

        // Act & Assert
        FluentActions.Invoking(() => DevelopmentDataSeeder.Seed(app))
            .Should().NotThrow<InvalidOperationException>("porque em ambiente de testes o método retorna antes de tentar semear");
    }
}
