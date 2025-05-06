using Bouchonnois.Domain;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Domain.Common.Errors;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.UseCases.PrendreLApéro;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro : UseCaseTest
{
    private readonly UseCase _useCase;

    public PrendreLApéro() => _useCase = new UseCase(Repository, () => Now);

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        _useCase.Handle(new Command(id))
            .Should()
            .Succeed();

        Repository.PartieDeChasseSauvegardée()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(new ApéroDémarré(Now));
    }

    [Fact]
    public void EchoueCarPartieNExistePas()
    {
        var id = UnePartieDeChasseInexistante();

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(LaPartieDeChasseNexistePas());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApéro()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(OnEstDéjàEnTrainDePrendreLApéro());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(OnNePrendPasLapéroQuandLaPartieEstTerminée());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }
}