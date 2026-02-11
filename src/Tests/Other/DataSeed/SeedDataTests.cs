using Domain.Cadastros.Aggregates;
using Domain.Identidade.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Tests.Application.SharedHelpers.AggregateBuilders;

namespace Tests.Other.DataSeed
{
    public class SeedDataTests
    {
        private readonly SeedDataTestFixture _fixture;

        public SeedDataTests()
        {
            _fixture = new SeedDataTestFixture();
        }

        #region SeedClientes Tests

        [Fact(DisplayName = "Deve popular 6 clientes quando banco estiver vazio")]
        [Trait("Método", "SeedClientes")]
        public void SeedClientes_Deve_PopularClientes_Quando_BancoVazio()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            context.Clientes.Should().BeEmpty();

            // Act
            SeedData.SeedClientes(context);

            // Assert
            var clientes = context.Clientes.ToList();
            clientes.Should().HaveCount(6);
            clientes.Should().Contain(c => c.Nome.Valor == "João Silva" && c.DocumentoIdentificador.Valor == "56229071010");
            clientes.Should().Contain(c => c.Nome.Valor == "Maria Santos" && c.DocumentoIdentificador.Valor == "99754534063");
            clientes.Should().Contain(c => c.Nome.Valor == "Pedro Oliveira" && c.DocumentoIdentificador.Valor == "13763122044");
            clientes.Should().Contain(c => c.Nome.Valor == "Transportadora Logística Express Ltda" && c.DocumentoIdentificador.Valor == "62255092000108");
            clientes.Should().Contain(c => c.Nome.Valor == "Auto Peças e Serviços São Paulo S.A." && c.DocumentoIdentificador.Valor == "13179173000160");
            clientes.Should().Contain(c => c.Nome.Valor == "cliente" && c.DocumentoIdentificador.Valor == "19649323007");
        }

        [Fact(DisplayName = "Não deve popular clientes quando já existirem dados")]
        [Trait("Método", "SeedClientes")]
        public void SeedClientes_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            var clienteExistente = new ClienteBuilder()
                .ComNome("Cliente Existente")
                .ComDocumento("56229071010")
                .Build();
            context.Clientes.Add(clienteExistente);
            context.SaveChanges();

            var quantidadeInicial = context.Clientes.Count();

            // Act
            SeedData.SeedClientes(context);

            // Assert
            var quantidadeFinal = context.Clientes.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            context.Clientes.Should().Contain(c => c.Nome.Valor == "Cliente Existente");
        }

        #endregion

        #region SeedVeiculos Tests

        [Fact(DisplayName = "Deve popular 5 veículos quando banco estiver vazio e houver 5+ clientes")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_Deve_PopularVeiculos_Quando_BancoVazioEHouverClientes()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            SeedData.SeedClientes(context);
            context.Veiculos.Should().BeEmpty();

            // Act
            SeedData.SeedVeiculos(context);

            // Assert
            var veiculos = context.Veiculos.ToList();
            veiculos.Should().HaveCount(5);
            veiculos.Should().Contain(v => v.Placa.Valor == "ABC1234" && v.Modelo.Valor == "Civic");
            veiculos.Should().Contain(v => v.Placa.Valor == "XYZ5678" && v.Modelo.Valor == "Corolla");
            veiculos.Should().Contain(v => v.Placa.Valor == "DEF9012" && v.Modelo.Valor == "CB 600F");
            veiculos.Should().Contain(v => v.Placa.Valor == "GHI3456" && v.Modelo.Valor == "Onix");
            veiculos.Should().Contain(v => v.Placa.Valor == "JKL7890" && v.Modelo.Valor == "YZF-R3");
        }

        [Fact(DisplayName = "Não deve popular veículos quando não houver clientes suficientes")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_NaoDevePopular_Quando_NaoHouverClientesSuficientes()
        {
            // Arrange - Contexto com apenas 2 clientes (menos que os 5 necessários)
            using var context = _fixture.CriarDbComMenosDe5Clientes();

            // Act
            SeedData.SeedVeiculos(context);

            // Assert
            context.Veiculos.Should().BeEmpty();
        }

        [Fact(DisplayName = "Não deve popular veículos quando já existirem dados")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            SeedData.SeedClientes(context);
            var cliente = context.Clientes.First();
            var veiculoExistente = new VeiculoBuilder()
                .ComClienteId(cliente.Id)
                .ComPlaca("XXX-1111")
                .ComModelo("Modelo Teste")
                .Build();
            context.Veiculos.Add(veiculoExistente);
            context.SaveChanges();

            var quantidadeInicial = context.Veiculos.Count();

            // Act
            SeedData.SeedVeiculos(context);

            // Assert
            var quantidadeFinal = context.Veiculos.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            context.Veiculos.Should().Contain(v => v.Placa.Valor == "XXX1111");
        }

        #endregion

        #region SeedServicos Tests

        [Fact(DisplayName = "Deve popular 8 serviços quando banco estiver vazio")]
        [Trait("Método", "SeedServicos")]
        public void SeedServicos_Deve_PopularServicos_Quando_BancoVazio()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            context.Servicos.Should().BeEmpty();

            // Act
            SeedData.SeedServicos(context);

            // Assert
            var servicos = context.Servicos.ToList();
            servicos.Should().HaveCount(8);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Óleo" && s.Preco.Valor == 80.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Alinhamento e Balanceamento" && s.Preco.Valor == 120.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Revisão Completa" && s.Preco.Valor == 350.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Pastilhas de Freio" && s.Preco.Valor == 180.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Filtro de Ar" && s.Preco.Valor == 45.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Diagnóstico Eletrônico" && s.Preco.Valor == 100.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Correia Dentada" && s.Preco.Valor == 280.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Limpeza de Bicos Injetores" && s.Preco.Valor == 150.00m);
        }

        [Fact(DisplayName = "Não deve popular serviços quando já existirem dados")]
        [Trait("Método", "SeedServicos")]
        public void SeedServicos_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            var servicoExistente = new ServicoBuilder()
                .ComNome("Serviço Existente")
                .ComPreco(50.00m)
                .Build();
            context.Servicos.Add(servicoExistente);
            context.SaveChanges();

            var quantidadeInicial = context.Servicos.Count();

            // Act
            SeedData.SeedServicos(context);

            // Assert
            var quantidadeFinal = context.Servicos.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            context.Servicos.Should().Contain(s => s.Nome.Valor == "Serviço Existente");
        }

        #endregion

        #region SeedUsuarios Tests

        [Fact(DisplayName = "Deve popular 2 roles e 2 usuários quando banco estiver vazio")]
        [Trait("Método", "SeedUsuarios")]
        public void SeedUsuarios_Deve_PopularUsuarios_Quando_BancoVazio()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            context.Usuarios.Should().BeEmpty();

            // Act
            SeedData.SeedUsuarios(context);

            // Assert
            // Verificar roles criadas
            var roles = context.Roles.ToList();
            roles.Should().Contain(r => r.Id == RoleEnum.Administrador);
            roles.Should().Contain(r => r.Id == RoleEnum.Cliente);

            // Verificar usuários criados
            var usuarios = context.Usuarios.Include(u => u.Roles).ToList();
            usuarios.Should().HaveCount(2);

            // Verificar administrador
            var admin = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == "82954150009");
            admin.Should().NotBeNull();
            admin!.SenhaHash.Valor.Should().NotBeNullOrEmpty();
            admin.Roles.Should().HaveCount(1);
            admin.Roles.First().Id.Should().Be(RoleEnum.Administrador);

            // Verificar cliente
            var cliente = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == "19649323007");
            cliente.Should().NotBeNull();
            cliente!.SenhaHash.Valor.Should().NotBeNullOrEmpty();
            cliente.Roles.Should().HaveCount(1);
            cliente.Roles.First().Id.Should().Be(RoleEnum.Cliente);
        }

        [Fact(DisplayName = "Não deve popular usuários quando já existirem dados")]
        [Trait("Método", "SeedUsuarios")]
        public void SeedUsuarios_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            SeedData.SeedUsuarios(context);
            var usuariosIniciais = context.Usuarios.Count();

            // Act
            SeedData.SeedUsuarios(context);

            // Assert
            context.Usuarios.Should().HaveCount(usuariosIniciais);
        }

        #endregion

        #region SeedAll Tests

        [Fact(DisplayName = "Deve popular todos os dados quando banco estiver vazio")]
        [Trait("Método", "SeedAll")]
        public void SeedAll_Deve_PopularTodosDados_Quando_BancoVazio()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();
            context.Usuarios.Should().BeEmpty();
            context.Clientes.Should().BeEmpty();
            context.Veiculos.Should().BeEmpty();
            context.Servicos.Should().BeEmpty();

            // Act
            SeedData.SeedAll(context);

            // Assert
            context.Usuarios.Should().HaveCount(2);
            context.Clientes.Should().HaveCount(6);
            context.Veiculos.Should().HaveCount(5);
            context.Servicos.Should().HaveCount(8);
        }

        [Fact(DisplayName = "Deve chamar métodos de seed na ordem correta garantindo dependências")]
        [Trait("Método", "SeedAll")]
        public void SeedAll_DeveChamarMetodos_NaOrdemCorreta()
        {
            // Arrange
            using var context = _fixture.CriarDbContext();

            // Act
            SeedData.SeedAll(context);

            // Assert
            // Veículos só são criados se há clientes suficientes
            // Isso confirma que SeedClientes foi chamado antes de SeedVeiculos
            var veiculos = context.Veiculos.ToList();
            veiculos.Should().HaveCount(5);

            // Verificar se os veículos estão associados aos clientes seeded
            var clientesIds = context.Clientes.Select(c => c.Id).ToList();
            veiculos.Should().OnlyContain(v => clientesIds.Contains(v.ClienteId));
        }

        #endregion
    }
}
