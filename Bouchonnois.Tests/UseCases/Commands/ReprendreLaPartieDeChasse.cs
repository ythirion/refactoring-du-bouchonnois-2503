using Bouchonnois.Domain;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Errors;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

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

        _reprendreLaPartieUseCase.Handle(id)
            .Should()
            .Succeed();

        Repository.SavedPartieDeChasse()
            .DevraitEtreEnCours()
            .DevraitAvoirEmis(Now, "Reprise de la chasse");
    }

    public class Failure : ReprendreLaPartieDeChasse
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = UnePartieDeChasseInexistante();

            var result = _reprendreLaPartieUseCase.Handle(id);

            result.Should().FailWith(UseCasesErrorMessages.LaPartieDeChasseNExistePas());

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaChasseEstEnCours()
        {
            var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

            var result = _reprendreLaPartieUseCase.Handle(id);

            result.Should().FailWith(Error.LaPartieDeChasseEstDejaEnCoursError());

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaPartieDeChasseEstTerminée()
        {
            var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

            var result = _reprendreLaPartieUseCase.Handle(id);

            result.Should().FailWith(Error.QuandCestFiniCestFiniError());

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}
