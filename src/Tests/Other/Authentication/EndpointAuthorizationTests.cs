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
        [InlineData("GET", "/api/cadastros/clientes")]
        [InlineData("GET", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/clientes/documento/12345678901")]
        [InlineData("POST", "/api/cadastros/clientes")]
        [InlineData("PUT", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        // ServicoController endpoints
        [InlineData("GET", "/api/cadastros/servicos")]
        [InlineData("GET", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/servicos")]
        [InlineData("PUT", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        // VeiculoController endpoints
        [InlineData("GET", "/api/cadastros/veiculos")]
        [InlineData("GET", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/veiculos/placa/ABC1234")]
        [InlineData("GET", "/api/cadastros/veiculos/cliente/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/veiculos")]
        [InlineData("PUT", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        // UsuarioController endpoints
        [InlineData("GET", "/api/identidade/usuarios/documento/12345678901")]
        [InlineData("POST", "/api/identidade/usuarios")]
        public async Task Endpoints_SemAutenticacao_DevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion
    }
}
