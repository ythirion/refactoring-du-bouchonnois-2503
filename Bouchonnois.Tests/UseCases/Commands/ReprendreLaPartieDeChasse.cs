using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Exceptions;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class ReprendreLaPartieDeChasse : UseCaseTest
{
    private readonly ReprendreLaPartieUseCase _reprendreLaPartieUseCase;
    public ReprendreLaPartieDeChasse()
    {
        _reprendreLaPartieUseCase = new ReprendreLaPartieUseCase(Repository, () => Now);
    }
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro());

        _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreEnCours()
            .DevraitAvoirEmis(Now, "Reprise de la chasse");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var reprendrePartieQuandPartieExistePas = () => _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var reprendreLaPartieQuandChasseEnCours = () => _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        reprendreLaPartieQuandChasseEnCours.Should().Throw<LaChasseEstDéjàEnCours>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _reprendreLaPartieUseCase.ReprendreLaPartie(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
