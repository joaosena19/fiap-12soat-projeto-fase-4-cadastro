using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Identidade.Aggregates;
using Infrastructure.Authentication.PasswordHashing;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Seed;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static void SeedClientes(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Clientes.Any())
            {
                return;
            }

            // 2. Cria dados de teste para clientes
            var clientesDeTeste = new List<Cliente>
            {
                Cliente.Criar("João Silva", "56229071010"),
                Cliente.Criar("Maria Santos", "99754534063"),
                Cliente.Criar("Pedro Oliveira", "13763122044"),
                Cliente.Criar("Transportadora Logística Express Ltda", "62255092000108"),
                Cliente.Criar("Auto Peças e Serviços São Paulo S.A.", "13179173000160"),
                Cliente.Criar("cliente", "19649323007") // Cliente com usuário
            };

            // 3. Salva os dados no banco
            context.Clientes.AddRange(clientesDeTeste);
            context.SaveChanges();
        }

        public static void SeedVeiculos(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Veiculos.Any())
                return;

            // Obtém alguns clientes existentes para associar aos veículos
            var clientes = context.Clientes.Take(5).ToList();
            if (clientes.Count < 5)
                return; // Se não há clientes suficientes, não cria veículos

            // 2. Cria dados de teste para veículos
            var veiculosDeTeste = new List<Veiculo>
            {
                Veiculo.Reidratar(SeedIds.Veiculos.Abc1234, clientes[0].Id, "ABC-1234", "Civic", "Honda", "Prata", 2020, TipoVeiculoEnum.Carro),
                Veiculo.Reidratar(SeedIds.Veiculos.Xyz5678, clientes[1].Id, "XYZ-5678", "Corolla", "Toyota", "Branco", 2019, TipoVeiculoEnum.Carro),
                Veiculo.Reidratar(SeedIds.Veiculos.Def9012, clientes[2].Id, "DEF-9012", "CB 600F", "Honda", "Azul", 2021, TipoVeiculoEnum.Moto),
                Veiculo.Criar(clientes[3].Id, "GHI-3456", "Onix", "Chevrolet", "Vermelho", 2022, TipoVeiculoEnum.Carro),
                Veiculo.Criar(clientes[4].Id, "JKL-7890", "YZF-R3", "Yamaha", "Preto", 2020, TipoVeiculoEnum.Moto)
            };

            // 3. Salva os dados no banco
            context.Veiculos.AddRange(veiculosDeTeste);
            context.SaveChanges();
        }

        public static void SeedServicos(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Servicos.Any())
                return;

            // 2. Cria dados de teste para serviços
            var servicosDeTeste = new List<Servico>
            {
                Servico.Reidratar(SeedIds.Servicos.TrocaDeOleo, "Troca de Óleo", 80.00m),
                Servico.Reidratar(SeedIds.Servicos.AlinhamentoBalanceamento, "Alinhamento e Balanceamento", 120.00m),
                Servico.Reidratar(SeedIds.Servicos.RevisaoCompleta, "Revisão Completa", 350.00m),
                Servico.Criar("Troca de Pastilhas de Freio", 180.00m),
                Servico.Criar("Troca de Filtro de Ar", 45.00m),
                Servico.Criar("Diagnóstico Eletrônico", 100.00m),
                Servico.Criar("Troca de Correia Dentada", 280.00m),
                Servico.Criar("Limpeza de Bicos Injetores", 150.00m)
            };

            // 3. Salva os dados no banco
            context.Servicos.AddRange(servicosDeTeste);
            context.SaveChanges();
        }

        public static void SeedUsuarios(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Usuarios.Any())
                return;

            // 2. Configura PasswordHasher com opções padrão
            var options = new Argon2HashingOptions
            {
                SaltSize = 16,
                HashSize = 32,
                Iterations = 4,
                MemorySize = 65536,
                DegreeOfParallelism = 1
            };
            var passwordHasher = new PasswordHasher(Options.Create(options));

            // 3. Busca ou cria as roles se elas não existirem (para testes unitários)
            var roleAdmin = context.Roles.FirstOrDefault(r => r.Id == Domain.Identidade.Enums.RoleEnum.Administrador);
            if (roleAdmin == null)
            {
                roleAdmin = Role.Administrador();
                context.Roles.Add(roleAdmin);
            }

            var roleCliente = context.Roles.FirstOrDefault(r => r.Id == Domain.Identidade.Enums.RoleEnum.Cliente);
            if (roleCliente == null)
            {
                roleCliente = Role.Cliente();
                context.Roles.Add(roleCliente);
            }

            context.SaveChanges(); // Salva as roles primeiro se necessário

            // 4. Cria usuários de teste
            var usuariosDeTeste = new List<Usuario>
            {
                // Administrador
                Usuario.Criar("82954150009", passwordHasher.Hash("admin123"), roleAdmin),
                
                // Cliente 
                Usuario.Criar("19649323007", passwordHasher.Hash("cliente123"), roleCliente)
            };

            // 5. Salva os usuários no banco
            context.Usuarios.AddRange(usuariosDeTeste);
            context.SaveChanges();
        }

        public static void SeedAll(AppDbContext context)
        {
            SeedUsuarios(context);
            SeedClientes(context);
            SeedVeiculos(context);
            SeedServicos(context);
        }
    }
}
