using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro : UseCaseTest
{
    private readonly PrendreLapéroUseCase _useCase;

    public PrendreLApéro() => _useCase = new PrendreLapéroUseCase(Repository, () => Now);

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        _useCase.Handle(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(Now, "Petit apéro");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var apéroQuandPartieExistePas = () => _useCase.Handle(id);

        apéroQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var prendreLApéroQuandOnPrendDéjàLapéro = () => _useCase.Handle(id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should().Throw<OnEstDéjàEnTrainDePrendreLapéro>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _useCase.Handle(id);

        prendreLapéroQuandTerminée.Should().Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}