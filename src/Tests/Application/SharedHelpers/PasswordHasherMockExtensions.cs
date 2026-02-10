using Application.Contracts.Services;
using Moq;

namespace Tests.Application.SharedHelpers
{
    public static class PasswordHasherMockExtensions
    {
        public static void DeveTerHasheadoSenha(this Mock<IPasswordHasher> mock, string senha)
        {
            mock.Verify(ph => ph.Hash(senha), Times.Once,
                $"Era esperado que o método Hash fosse chamado exatamente uma vez com a senha '{senha}'.");
        }

        public static void DeveTerHasheadoQualquerSenha(this Mock<IPasswordHasher> mock)
        {
            mock.Verify(ph => ph.Hash(It.IsAny<string>()), Times.Once,
                "Era esperado que o método Hash fosse chamado exatamente uma vez com qualquer senha.");
        }

        public static void NaoDeveTerHasheadoNenhumaSenha(this Mock<IPasswordHasher> mock)
        {
            mock.Verify(ph => ph.Hash(It.IsAny<string>()), Times.Never,
                "O método Hash não deveria ter sido chamado.");
        }
    }
}
