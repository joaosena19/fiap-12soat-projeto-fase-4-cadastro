using FluentAssertions;
using Shared.Seed;

namespace Tests.Other.DataSeed;

public class SeedIdsTests
{
    [Fact(DisplayName = "Deve conter IDs determinísticos para itens de estoque quando acessado")]
    [Trait("Other", "SeedIds")]
    public void SeedIds_ItensEstoque_DeveConterIdsDeterministicos_QuandoAcessado()
    {
        // Act
        var oleoMotor5w30 = SeedIds.ItensEstoque.OleoMotor5w30;
        var filtroDeOleo = SeedIds.ItensEstoque.FiltroDeOleo;
        var pastilhaDeFreioDianteira = SeedIds.ItensEstoque.PastilhaDeFreioDianteira;

        // Assert
        oleoMotor5w30.Should().NotBe(Guid.Empty);
        oleoMotor5w30.Should().Be(Guid.Parse("4d444444-4444-4444-4444-444444444444"));

        filtroDeOleo.Should().NotBe(Guid.Empty);
        filtroDeOleo.Should().Be(Guid.Parse("5e555555-5555-5555-5555-555555555555"));

        pastilhaDeFreioDianteira.Should().NotBe(Guid.Empty);
        pastilhaDeFreioDianteira.Should().Be(Guid.Parse("6f666666-6666-6666-6666-666666666666"));
    }
}
