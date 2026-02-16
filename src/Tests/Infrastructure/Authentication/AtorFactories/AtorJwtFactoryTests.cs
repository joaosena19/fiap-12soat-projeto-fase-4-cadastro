using Domain.Identidade.Enums;
using FluentAssertions;
using Infrastructure.Authentication.AtorFactories;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Helpers;
using Xunit;

namespace Tests.Infrastructure.Authentication.AtorFactories
{
    public class AtorJwtFactoryTests
    {
        #region Token Inválido

        [Fact(DisplayName = "Deve lançar exceção quando token for inválido")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarExcecao_QuandoTokenInvalido()
        {
            // Arrange
            var tokenInvalido = "token.invalido.aqui";

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenInvalido))
                .Should().ThrowExactly<ArgumentException>()
                .Where(ex => ex.Message.Contains("Unable to decode"));
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando token for vazio")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoTokenVazio()
        {
            // Arrange
            var tokenVazio = "";

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenVazio))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token JWT inválido");
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando token for null")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoTokenNull()
        {
            // Arrange
            string tokenNull = null!;

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenNull))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token JWT inválido");
        }

        #endregion

        #region UserId Ausente ou Inválido

        [Fact(DisplayName = "Deve lançar Unauthorized quando userId estiver ausente")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoUserIdAusente()
        {
            // Arrange
            var token = new JwtTokenBuilder()
                .ComRole(RoleEnum.Administrador)
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token deve conter userId válido");
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando userId for inválido")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoUserIdInvalido()
        {
            // Arrange
            var token = new JwtTokenBuilder()
                .ComClaimCustomizada("userId", "id-invalido")
                .ComRole(RoleEnum.Administrador)
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token deve conter userId válido");
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando userId for vazio")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoUserIdVazio()
        {
            // Arrange
            var token = new JwtTokenBuilder()
                .ComClaimCustomizada("userId", "")
                .ComRole(RoleEnum.Administrador)
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token deve conter userId válido");
        }

        #endregion

        #region Roles Ausentes ou Inválidas

        [Fact(DisplayName = "Deve lançar Unauthorized quando roles estiverem ausentes")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoRolesAusentes()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Token deve conter pelo menos uma role");
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando role for inválida")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoRoleInvalida()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComClaimCustomizada("role", "RoleInexistente")
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Role 'RoleInexistente' não é válida. Roles permitidas: Administrador, Cliente, Sistema");
        }

        [Fact(DisplayName = "Deve lançar Unauthorized quando uma das roles for inválida")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveLancarUnauthorized_QuandoUmaDasRolesForInvalida()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRole(RoleEnum.Administrador)
                .ComClaimCustomizada("role", "RoleInvalida")
                .Build();

            // Act & Assert
            FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(token))
                .Should().Throw<DomainException>()
                .Where(ex => ex.ErrorType == ErrorType.Unauthorized)
                .WithMessage("Role 'RoleInvalida' não é válida. Roles permitidas: Administrador, Cliente, Sistema");
        }

        #endregion

        #region Token Válido

        [Fact(DisplayName = "Deve criar ator quando token for válido com role Administrador")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveCriarAtor_QuandoTokenValidoComAdministrador()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRole(RoleEnum.Administrador)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
            ator.Roles.Should().ContainSingle()
                .Which.Should().Be(RoleEnum.Administrador);
        }

        [Fact(DisplayName = "Deve criar ator quando token for válido com role Cliente")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveCriarAtor_QuandoTokenValidoComCliente()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRole(RoleEnum.Cliente)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
            ator.Roles.Should().ContainSingle()
                .Which.Should().Be(RoleEnum.Cliente);
        }

        [Fact(DisplayName = "Deve criar ator quando token for válido com role Sistema")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveCriarAtor_QuandoTokenValidoComSistema()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRole(RoleEnum.Sistema)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
            ator.Roles.Should().ContainSingle()
                .Which.Should().Be(RoleEnum.Sistema);
        }

        [Fact(DisplayName = "Deve criar ator quando token for válido com múltiplas roles")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveCriarAtor_QuandoTokenValidoComMultiplasRoles()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRoles(RoleEnum.Administrador, RoleEnum.Cliente)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
            ator.Roles.Should().HaveCount(2);
            ator.Roles.Should().Contain(RoleEnum.Administrador);
            ator.Roles.Should().Contain(RoleEnum.Cliente);
        }

        #endregion

        #region ClienteId Opcional

        [Fact(DisplayName = "Deve popular clienteId quando claim clienteId estiver presente e válida")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DevePopularClienteId_QuandoClaimClienteIdValida()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComClienteId(clienteId)
                .ComRole(RoleEnum.Cliente)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().Be(clienteId);
            ator.Roles.Should().ContainSingle()
                .Which.Should().Be(RoleEnum.Cliente);
        }

        [Fact(DisplayName = "Deve manter clienteId null quando claim clienteId estiver ausente")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveManterClienteIdNull_QuandoClaimClienteIdAusente()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComRole(RoleEnum.Administrador)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
        }

        [Fact(DisplayName = "Deve manter clienteId null quando claim clienteId for inválida")]
        [Trait("Factory", "AtorJwtFactory")]
        public void CriarAtor_DeveManterClienteIdNull_QuandoClaimClienteIdInvalida()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var token = new JwtTokenBuilder()
                .ComUsuarioId(usuarioId)
                .ComClaimCustomizada("clienteId", "id-invalido")
                .ComRole(RoleEnum.Cliente)
                .Build();

            // Act
            var ator = AtorJwtFactory.CriarPorTokenJwt(token);

            // Assert
            ator.Should().NotBeNull();
            ator.UsuarioId.Should().Be(usuarioId);
            ator.ClienteId.Should().BeNull();
        }

        #endregion
    }
}
