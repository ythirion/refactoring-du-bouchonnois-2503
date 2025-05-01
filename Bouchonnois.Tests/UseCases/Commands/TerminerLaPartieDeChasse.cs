using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Errors;

using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class TerminerLaPartieDeChasse : UseCaseTest
{
    private readonly TerminerLaPartieUseCase _sut;
    public TerminerLaPartieDeChasse()
    {
        _sut = new TerminerLaPartieUseCase(Repository, () => Now);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(
                    Dédé().Brocouille(),
                    Bernard().Brocouille(),
                    Robert().AyantCapturéGalinettes(2)));

        _sut.Handle(id)
            .Should()
            .SucceedWith(Robert);

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Robert().AyantCapturéGalinettes(2)));

        _sut.Handle(id)
            .Should()
            .SucceedWith(Robert);

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt2ChasseursExAequo()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(
                    Dédé().AyantCapturéGalinettes(2),
                    Bernard().AyantCapturéGalinettes(2),
                    Robert().Brocouille()));

        _sut.Handle(id)
            .Should()
            .SucceedWith("Dédé, Bernard");

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(
                Now,
                "La partie de chasse est terminée, vainqueur : Dédé - 2 galinettes, Bernard - 2 galinettes");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEtToutLeMondeBrocouille()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(
                    Dédé().Brocouille(),
                    Bernard().Brocouille(),
                    Robert().Brocouille()));

        _sut.Handle(id)
            .Should()
            .SucceedWith("Brocouille");

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(
                Now,
                "La partie de chasse est terminée, vainqueur : Brocouille");
    }

    [Fact]
    public void QuandLesChasseursSontALaperoEtTousExAequo()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro()
                .Avec(
                    Dédé().AyantCapturéGalinettes(3),
                    Bernard().AyantCapturéGalinettes(3),
                    Robert().AyantCapturéGalinettes(3)));

        _sut.Handle(id)
            .Should()
            .SucceedWith("Dédé, Bernard, Robert");

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(
                Now,
                "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes");
    }

    public class Failure : TerminerLaPartieDeChasse
    {
        [Fact]
        public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
        {
            var id = UnePartieDeChasseExistante(
                UnePartieDeChasse()
                    .Terminée());

            var result = _sut.Handle(id);

            result.Should().FailWith(new Error(DomainErrorMessages.QuandCestFiniCestFini));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaPartieDeChasseNExsitePas()
        {
            var id = UnePartieDeChasseInexistante();

            var result = _sut.Handle(id);

            result.Should().FailWith(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}
