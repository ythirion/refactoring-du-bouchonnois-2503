using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Errors;

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

        _prendereLAperoUseCase
            .Handle(id)
            .Should()
            .Succeed();

        Repository.SavedPartieDeChasse()
            .DevraitEtreALApéro()
            .DevraitAvoirEmis(Now, "Petit apéro");
    }

    public class Failure : PrendreLApéro
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = UnePartieDeChasseInexistante();

            _prendereLAperoUseCase
                .Handle(id)
                .Should()
                .FailWith(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLesChasseursSontDéjaEnApero()
        {
            var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

            _prendereLAperoUseCase
                .Handle(id)
                .Should()
                .FailWith(new Error(DomainErrorMessages.OnEstDéjàEnTrainDePrendreLApéro));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaPartieDeChasseEstTerminée()
        {
            var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

            _prendereLAperoUseCase
                .Handle(id)
                .Should()
                .FailWith(new Error(DomainErrorMessages.OnNePrendPasLApéroQuandLaPartieEstTerminée));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}
