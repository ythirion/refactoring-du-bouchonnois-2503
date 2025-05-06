using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class PrendreLApéro : UseCaseTest
{
    private readonly PrendreLAperoUseCase _prendreLAperoUseCase;

    public PrendreLApéro()
    {
        _prendreLAperoUseCase = new PrendreLAperoUseCase(
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

        _prendreLAperoUseCase.Handle(id);

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(Now, "Petit apéro");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        _prendreLAperoUseCase.HandleWithoutException(id)
            .ConvertFailure<LaPartieDeChasseNexistePas>().Should().BeOfType<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        _prendreLAperoUseCase.HandleWithoutException(id)
            .Should()
            .FailWith(new Error("On est déjà en train de prendre l'apéro"));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var prendreLapéroQuandTerminée = _prendreLAperoUseCase.HandleWithoutException(id);

        prendreLapéroQuandTerminée.Should().FailWith(new Error("On ne prend pas l'apéro quand la partie est terminée"));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
