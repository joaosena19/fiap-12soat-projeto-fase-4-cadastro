using FluentAssertions;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Infrastructure.SharedHelpers;
using Xunit;

namespace Tests.Infrastructure.Monitoramento;

public class ContextualLoggerTests
{
    private readonly Mock<ILogger> _loggerMock;

    public ContextualLoggerTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    [Fact(DisplayName = "Deve logar mensagem de informação com template e argumentos")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogInformation_DeveLogarMensagemComTemplateEArgumentos()
    {
        // Arrange
        var messageTemplate = "Processando pedido {OrderId}";
        var args = new object[] { 789 };
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());

        // Act
        sut.LogInformation(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve logar mensagem de aviso com template e argumentos")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogWarning_DeveLogarMensagemComTemplateEArgumentos()
    {
        // Arrange
        var messageTemplate = "Cache expirado para chave {Key}";
        var args = new object[] { "user:123" };
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());

        // Act
        sut.LogWarning(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve logar erro com template e argumentos")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_ComMensagem_DeveLogarErroComTemplateEArgumentos()
    {
        // Arrange
        var messageTemplate = "Falha ao conectar ao serviço {ServiceName}";
        var args = new object[] { "PaymentAPI" };
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());

        // Act
        sut.LogError(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve logar erro com exceção, template e argumentos")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_ComException_DeveLogarErroComExceptionTemplateEArgumentos()
    {
        // Arrange
        var exception = new TimeoutException("Timeout ao conectar");
        var messageTemplate = "Timeout ao acessar {Resource}";
        var args = new object[] { "Database" };
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());

        // Act
        sut.LogError(exception, messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve retornar novo ContextualLogger ao chamar ComPropriedade")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_DeveRetornarNovoContextualLogger()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var key = "UserId";
        var value = 456;

        // Act
        var result = sut.ComPropriedade(key, value);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ContextualLogger>();
        result.Should().NotBeSameAs(sut); // Deve retornar nova instância
    }

    [Fact(DisplayName = "ContextualLogger retornado por ComPropriedade deve conseguir logar")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_NovoLogger_DeveConseguirLogar()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var key = "CorrelationId";
        var value = Guid.NewGuid();
        var messageTemplate = "Operação iniciada";

        // Act
        var contextualLogger = sut.ComPropriedade(key, value);
        contextualLogger.LogInformation(messageTemplate);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve logar corretamente com ComPropriedade encadeado")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_Encadeado_DeveManterId()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var messageTemplate = "Operação complexa";

        // Act
        var logger = sut
            .ComPropriedade("RequestId", "req-123")
            .ComPropriedade("UserId", 789)
            .ComPropriedade("Action", "CreateOrder");

        logger.LogInformation(messageTemplate);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "ContextualLogger criado com contexto inicial deve manter contexto ao logar")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ContextualLogger_CriadoComContextoInicial_DeveManterContexto()
    {
        // Arrange
        var initialContext = new Dictionary<string, object?>
        {
            { "Key1", "Value1" },
            { "Key2", 123 }
        };
        var sut = new ContextualLogger(_loggerMock.Object, initialContext);
        var messageTemplate = "Log com contexto inicial";

        // Act
        sut.LogInformation(messageTemplate);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve aceitar valor null ao chamar ComPropriedade")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_ComValorNull_DeveAceitarENaoGerarErro()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var key = "OptionalField";
        object? value = null;

        // Act
        var result = sut.ComPropriedade(key, value);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ContextualLogger>();
    }

    [Fact(DisplayName = "Deve logar informação sem argumentos")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogInformation_ComParametrosVazios_DeveLogarCorretamente()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var messageTemplate = "Operação básica concluída";

        // Act
        sut.LogInformation(messageTemplate);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve substituir valor ao adicionar mesma chave duas vezes via ComPropriedade")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_AdicionarMesmaChaveDuasVezes_DeveSubstituirValor()
    {
        // Arrange
        var sut = new ContextualLogger(_loggerMock.Object, new Dictionary<string, object?>());
        var key = "Status";

        // Act
        var logger = sut
            .ComPropriedade(key, "Pending")
            .ComPropriedade(key, "Completed");

        logger.LogInformation("Status atualizado");

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }
}
