using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Tests.Integration;

namespace Tests.Other.Authentication
{
    public class EndpointAuthorizationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EndpointAuthorizationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Usa client sem autenticação
        }

        #region Endpoints que precisam de Authorize

        [Theory]
        // ClienteController endpoints
        [InlineData("GET", "/api/clientes")]
        [InlineData("GET", "/api/clientes/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/clientes/documento/12345678901")]
        [InlineData("POST", "/api/clientes")]
        [InlineData("PUT", "/api/clientes/00000000-0000-0000-0000-000000000000")]
        // ServicoController endpoints
        [InlineData("GET", "/api/servicos")]
        [InlineData("GET", "/api/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/servicos")]
        [InlineData("PUT", "/api/servicos/00000000-0000-0000-0000-000000000000")]
        // VeiculoController endpoints
        [InlineData("GET", "/api/veiculos")]
        [InlineData("GET", "/api/veiculos/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/veiculos/placa/ABC1234")]
        [InlineData("GET", "/api/veiculos/cliente/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/veiculos")]
        [InlineData("PUT", "/api/veiculos/00000000-0000-0000-0000-000000000000")]
        // UsuarioController endpoints
        [InlineData("GET", "/api/usuarios/documento/12345678901")]
        [InlineData("POST", "/api/usuarios")]
        public async Task Endpoints_SemAutenticacao_DevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) || method.Equals("PUT", StringComparison.OrdinalIgnoreCase) || method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion
    }
}
