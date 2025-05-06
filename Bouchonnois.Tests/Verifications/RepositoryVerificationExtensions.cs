using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Verifications;

public static class RepositoryVerificationExtensions
{
    public static PartieDeChasseRepositoryForTests NeDevraitPasAvoirSauvegarderDePartieDeChasse(this PartieDeChasseRepositoryForTests repository)
    {
        repository.PartieDeChasseSauvegardée().Should().BeNull();
        return repository;
    }
}