using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse : UseCaseTest
{
    private readonly ReprendreLaPartieDeChasseUseCase _useCase;

    public ReprendreLaPartieDeChasse() => _useCase = new ReprendreLaPartieDeChasseUseCase(Repository, () => Now);

    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro());

        _useCase.ReprendreLaPartie(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreEnCours()
            .DevraitAvoirEmis(Now, "Reprise de la chasse");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var reprendrePartieQuandPartieExistePas = () => _useCase.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var reprendreLaPartieQuandChasseEnCours = () => _useCase.ReprendreLaPartie(id);

        reprendreLaPartieQuandChasseEnCours.Should().Throw<LaChasseEstDéjàEnCours>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _useCase.ReprendreLaPartie(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}