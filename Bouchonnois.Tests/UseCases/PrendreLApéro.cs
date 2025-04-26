using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro : UseCaseTest
{
    private readonly PrendreLAperoUseCase _prendereLAperoUseCase;
    public PrendreLApéro()
    {
        _prendereLAperoUseCase = new PrendreLAperoUseCase(
            Repository,
            () => Now
        );
    }
    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        _prendereLAperoUseCase.PrendreLapéro(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(Now, "Petit apéro");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var apéroQuandPartieExistePas = () => _prendereLAperoUseCase.PrendreLapéro(id);

        apéroQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var prendreLApéroQuandOnPrendDéjàLapéro = () => _prendereLAperoUseCase.PrendreLapéro(id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should().Throw<OnEstDéjàEnTrainDePrendreLapéro>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _prendereLAperoUseCase.PrendreLapéro(id);

        prendreLapéroQuandTerminée.Should().Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
