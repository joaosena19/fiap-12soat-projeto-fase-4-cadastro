using FluentAssertions;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Exceptions;
using System.Net;
using System.Text.Json;
using Tests.Infrastructure.SharedHelpers;
using Tests.Integration.Middleware.Helpers;

namespace Tests.Integration.Middleware;

/// <summary>
/// Testes unitários para ExceptionHandlingMiddleware
/// </summary>
public class ExceptionHandlingMiddlewareTests : IClassFixture<ExceptionHandlingMiddlewareTestFixture>
{
    private readonly ExceptionHandlingMiddlewareTestFixture _fixture;

    public ExceptionHandlingMiddlewareTests(ExceptionHandlingMiddlewareTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Quando ocorre DomainException, deve retornar o status code mapeado, resposta JSON e logar Warning")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_QuandoDomainException_DeveRetornarStatusCodeMapeadoELogarWarning()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Cliente não encontrado";
        var errorType = ErrorType.ResourceNotFound;
        var domainException = new DomainException(mensagemErro, errorType);
        var expectedStatusCode = (int)HttpStatusCode.NotFound;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode, "o status code deve ser mapeado a partir do ErrorType");
        context.Response.ContentType.Should().Be("application/json", "o content-type deve ser JSON");

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(mensagemErro, "a mensagem deve ser a mensagem da DomainException");
        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode, "o statusCode no JSON deve corresponder ao status HTTP");

        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com ErrorType.InvalidInput, deve retornar 400 Bad Request")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_QuandoDomainExceptionComInvalidInput_DeveRetornar400()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Dados de entrada inválidos";
        var domainException = new DomainException(mensagemErro, ErrorType.InvalidInput);
        var expectedStatusCode = (int)HttpStatusCode.BadRequest;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com ErrorType.Conflict, deve retornar 409 Conflict")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_QuandoDomainExceptionComConflict_DeveRetornar409()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Recurso já existe";
        var domainException = new DomainException(mensagemErro, ErrorType.Conflict);
        var expectedStatusCode = (int)HttpStatusCode.Conflict;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre exceção inesperada, deve retornar 500 com mensagem padrão e logar Error")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_QuandoExcecaoInesperada_DeveRetornar500ELogarError()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var unexpectedException = new InvalidOperationException("Erro inesperado no sistema");
        var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
        var expectedMessage = "Ocorreu um erro interno no servidor.";

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, unexpectedException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode, "exceções não-domain devem retornar 500 Internal Server Error");
        context.Response.ContentType.Should().Be("application/json", "o content-type deve ser JSON");

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(expectedMessage, "a mensagem deve ser a mensagem padrão para erros internos");
        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode, "o statusCode no JSON deve ser 500");

        _fixture.LoggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Quando ocorre ArgumentNullException, deve retornar 500 e logar Error")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_QuandoArgumentNullException_DeveRetornar500ELogarError()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var exception = new ArgumentNullException("parametro", "Parâmetro obrigatório não fornecido");
        var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
        var expectedMessage = "Ocorreu um erro interno no servidor.";

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, exception);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(expectedMessage);
        _fixture.LoggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Resposta JSON deve estar em camelCase")]
    [Trait("Middleware", "ExceptionHandling")]
    public async Task InvokeAsync_RespostaJSON_DeveEstarEmCamelCase()
    {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var domainException = new DomainException("Teste", ErrorType.InvalidInput);

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        
        responseBody.Should().Contain("\"message\":", "a propriedade deve estar em camelCase (message, não Message)");
        responseBody.Should().Contain("\"statusCode\":", "a propriedade deve estar em camelCase (statusCode, não StatusCode)");
    }
}
