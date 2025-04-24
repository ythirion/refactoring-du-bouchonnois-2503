using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;
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

        _useCase.Handle(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreEnCours()
            .DevraitAvoirEmis(Now, "Reprise de la chasse");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var reprendrePartieQuandPartieExistePas = () => _useCase.Handle(id);

        reprendrePartieQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var reprendreLaPartieQuandChasseEnCours = () => _useCase.Handle(id);

        reprendreLaPartieQuandChasseEnCours.Should().Throw<LaChasseEstDéjàEnCours>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _useCase.Handle(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}