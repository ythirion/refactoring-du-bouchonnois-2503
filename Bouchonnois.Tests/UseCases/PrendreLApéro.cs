using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro : UseCaseTest
{
    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        Service.PrendreLapéro(id);

        Repository.SavedPartieDeChasse()
            .EstALApéro()
            .AEmisEvenement(Now, "Petit apéro");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var apéroQuandPartieExistePas = () => Service.PrendreLapéro(id);

        apéroQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var prendreLApéroQuandOnPrendDéjàLapéro = () => Service.PrendreLapéro(id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should().Throw<OnEstDéjàEnTrainDePrendreLapéro>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => Service.PrendreLapéro(id);

        prendreLapéroQuandTerminée.Should().Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}