using Bouchonnois.Domain;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.UseCases.PrendreLApero;

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
            .DevraitAvoirEmis(new ApéroDemarée(Now));
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(new Error("La partie de chasse n'existe pas"));

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(new Error("On est déjà en train de prendre l'apéro"));

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        _useCase.Handle(new Command(id))
            .Should()
            .FailWith(new Error("On ne prend pas l'apéro quand la partie est terminée"));

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }
}