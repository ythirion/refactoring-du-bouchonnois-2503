using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class PrendreLApéro : UseCaseTest
{
    private readonly PrendreLAperoUseCase _prendereLAperoUseCase;

    public PrendreLApéro()
    {
        _prendereLAperoUseCase = new PrendreLAperoUseCase(
            Repository,
            () => Now
        );
    }

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        _prendereLAperoUseCase.Handle(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(Now, "Petit apéro");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        _prendereLAperoUseCase.Handle(id)
            .Should()
            .FailWith(new Error("La partie de chasse n'existe pas"));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        _prendereLAperoUseCase.Handle(id)
            .Should()
            .FailWith(new Error("On est déjà en train de prendre l'apéro"));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = _prendereLAperoUseCase.Handle(id);

        prendreLapéroQuandTerminée.Should().FailWith(new Error("On ne prend pas l'apéro quand la partie est terminée"));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
