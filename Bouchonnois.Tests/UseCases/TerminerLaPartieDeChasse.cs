using Bouchonnois.Domain.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

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

        var meilleurChasseur = _sut.Handle(id);

        meilleurChasseur.Should().Be(Robert);

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

        var meilleurChasseur = _sut.Handle(id);

        meilleurChasseur.Should().Be(Robert);

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

        var meilleurChasseur = _sut.Handle(id);

        meilleurChasseur.Should().Be("Dédé, Bernard");

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

        var meilleurChasseur = _sut.Handle(id);

        meilleurChasseur.Should().Be("Brocouille");

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

        var meilleurChasseur = _sut.Handle(id);

        meilleurChasseur.Should().Be("Dédé, Bernard, Robert");

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(
                Now,
                "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .Terminée());

        var prendreLapéroQuandTerminée = () => _sut.Handle(id);

        prendreLapéroQuandTerminée.Should().Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
