using Bouchonnois.Domain.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

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

        _reprendreLaPartieUseCase.Handle(id);

        Repository.PartieDeChasseSauvegardée()
            .DevraitEtreEnCours()
            .DevraitAvoirEmis(Now, "Reprise de la chasse");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var reprendrePartieQuandPartieExistePas = () => _reprendreLaPartieUseCase.Handle(id);

        reprendrePartieQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.PartieDeChasseSauvegardée().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var reprendreLaPartieQuandChasseEnCours = () => _reprendreLaPartieUseCase.Handle(id);

        reprendreLaPartieQuandChasseEnCours.Should().Throw<LaChasseEstDéjàEnCours>();

        Repository.PartieDeChasseSauvegardée().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = () => _reprendreLaPartieUseCase.Handle(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.PartieDeChasseSauvegardée().Should().BeNull();
    }
}
