using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class TerminerLaPartieDeChasse
{
    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 2),
                }
            )
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(2);

        meilleurChasseur.Should().Be("Robert");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 2)
                }
            )
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(1);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(2);

        meilleurChasseur.Should().Be("Robert");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt2ChasseursExAequo()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20, nbGalinettes: 2),
                    new(nom: "Bernard", ballesRestantes: 8, nbGalinettes: 2),
                    new(nom: "Robert", ballesRestantes: 12),
                }
            )
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(2);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(2);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);

        meilleurChasseur.Should().Be("Dédé, Bernard");
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEtToutLeMondeBrocouille()
    {
        var now = DateTime.Now;
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                }
            )
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);

        meilleurChasseur.Should().Be("Brocouille");

        savedPartieDeChasse
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, "La partie de chasse est terminée, vainqueur : Brocouille")]);
    }

    [Fact]
    public void QuandLesChasseursSontALaperoEtTousExAequo()
    {
        var now = DateTime.Now;

        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstApero()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20, nbGalinettes: 3),
                    new(nom: "Bernard", ballesRestantes: 8, nbGalinettes: 3),
                    new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 3)
                }
            )
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(3);

        meilleurChasseur.Should().Be("Dédé, Bernard, Robert");

        savedPartieDeChasse
            .Events
            .Should()
            .BeEquivalentTo(
            [
                new Event(
                    now,
                    "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes")
            ]);
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminee()
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.TerminerLaPartie(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }
}
