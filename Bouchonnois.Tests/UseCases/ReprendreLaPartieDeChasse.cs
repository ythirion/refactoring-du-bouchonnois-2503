using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse : UseCaseTest
{
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro());

        Service.ReprendreLaPartie(id);

        Repository.SavedPartieDeChasse()
            .EstEnCours()
            .DoitAvoirEmis(Now, "Reprise de la chasse");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var reprendrePartieQuandPartieExistePas = () => Service.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var reprendreLaPartieQuandChasseEnCours = () => Service.ReprendreLaPartie(id);

        reprendreLaPartieQuandChasseEnCours.Should().Throw<LaChasseEstDéjàEnCours>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => Service.ReprendreLaPartie(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}