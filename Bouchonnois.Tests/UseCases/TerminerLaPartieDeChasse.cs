using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class TerminerLaPartieDeChasse : UseCaseTest
{
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

        var meilleurChasseur = Service.TerminerLaPartie(id);

        meilleurChasseur.Should().Be(Robert);

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Robert().AyantCapturéGalinettes(2)));

        var meilleurChasseur = Service.TerminerLaPartie(id);

        meilleurChasseur.Should().Be(Robert);

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes");
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

        var meilleurChasseur = Service.TerminerLaPartie(id);

        meilleurChasseur.Should().Be("Dédé, Bernard");

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(
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

        var meilleurChasseur = Service.TerminerLaPartie(id);

        meilleurChasseur.Should().Be("Brocouille");

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(
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
    

        var meilleurChasseur = Service.TerminerLaPartie(id);

        meilleurChasseur.Should().Be("Dédé, Bernard, Robert");

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(Now,
                "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasse(
            id: id,
            chasseurs: new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 8),
                new(nom: Robert, ballesRestantes: 12)
            },
            terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
            status: PartieStatus.Terminée);
        repository.Add(
            partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.TerminerLaPartie(id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }
}