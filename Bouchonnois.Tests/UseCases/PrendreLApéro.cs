using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using CSharpFunctionalExtensions;
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

        QuandPrendreLApéro(id)
            .Should()
            .Succeed();

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(new ApéroDémarré(Now));
    }

    [Fact]
    public void EchoueCarPartieNExistePas()
    {
        var id = UnePartieDeChasseInexistante();

        QuandPrendreLApéro(id)
            .Should()
            .FailWith(LaPartieDeChasseNexistePas());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApéro()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        QuandPrendreLApéro(id)
            .Should()
            .FailWith(OnEstDéjàEnTrainDePrendreLApéro());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        QuandPrendreLApéro(id)
            .Should()
            .FailWith(OnNePrendPasLapéroQuandLaPartieEstTerminée());

        Repository.NeDevraitPasAvoirSauvegarderDePartieDeChasse();
    }

    private UnitResult<Error> QuandPrendreLApéro(Guid id) => _useCase.Handle(new Request(id));
}