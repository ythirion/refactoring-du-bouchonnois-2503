using Bouchonnois.Domain;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class PrendreLApéro : UseCaseTest
{
    private readonly PrendreLAperoUseCase _prendreLAperoUseCase;

    public PrendreLApéro() => _prendreLAperoUseCase = new PrendreLAperoUseCase(Repository, () => Now);

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var partieId = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
        );

        partieId.Run(id => _prendreLAperoUseCase.HandleCommand(new PrendreLapéro(id)))
            .Succeed()
            .And(_ => Repository.SavedPartieDeChasse()
                .DevraitEtreALApéro()
                .DevraitAvoirEmis(Now, new ApéroADémarré(partieId, Now))
            );
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
        => UnePartieDeChasseInexistante()
            .Run(id => _prendreLAperoUseCase.Handle(id))
            .FailWith("La partie de chasse n'existe pas")
            .And(_ => Repository.NothingHasBeenSaved());

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
        => UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro())
            .Run(id => _prendreLAperoUseCase.Handle(id))
            .FailWith("On est déjà en train de prendre l'apéro")
            .And(_ => Repository.NothingHasBeenSaved());

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
        => UnePartieDeChasseExistante(UnePartieDeChasse().Terminée())
            .Run(id => _prendreLAperoUseCase.Handle(id))
            .FailWith("On ne prend pas l'apéro quand la partie est terminée")
            .And(_ => Repository.NothingHasBeenSaved());
}
