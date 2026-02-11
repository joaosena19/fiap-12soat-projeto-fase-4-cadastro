using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Tests.Application.SharedHelpers.AggregateBuilders;

namespace Tests.Other.DataSeed
{
    public class SeedDataTestFixture
    {
        /// <summary>
        /// Cria um novo contexto de banco de dados em memória.
        /// Cada contexto usa um banco de dados único (Guid.NewGuid()) para isolamento entre testes.
        /// </summary>
        public AppDbContext CriarDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        /// <summary>
        /// Cria um contexto com menos de 5 clientes para testar guard clauses.
        /// </summary>
        public AppDbContext CriarDbComMenosDe5Clientes()
        {
            var context = CriarDbContext();

            var clientes = new[]
            {
                new ClienteBuilder().ComNome("Cliente 1").ComDocumento("56229071010").Build(),
                new ClienteBuilder().ComNome("Cliente 2").ComDocumento("99754534063").Build()
            };

            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            return context;
        }
    }
}
