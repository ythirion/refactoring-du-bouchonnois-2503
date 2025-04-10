using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;

namespace Bouchonnois.Tests.UseCases;

public class TerminerLaPartieDeChasse : PartieDeChasseBaseTests
{
    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([
                Dédé, Bernard,
                new Chasseur(Data.Robert, 12, 2)
            ])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be(Data.Dédé);
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be(Data.Bernard);
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be(Data.Robert);
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(2);

        meilleurChasseur.Should().Be(Data.Robert);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([new Chasseur(Data.Robert, 12, 2)])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
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
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs(new List<Chasseur>
            {
                new(Data.Dédé, 20, 2),
                new(Data.Bernard, 8, 2),
                Robert
            })
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
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

        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.Terminée);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be(Data.Dédé);
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be(Data.Bernard);
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be(Data.Robert);
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

        var partieDeChasse = UnePartieDeChasseALApero
            .AvecDesChasseurs([
                new Chasseur(Data.Dédé, 20, 3),
                new Chasseur(Data.Bernard, 8, 3),
                new Chasseur(Data.Robert, 12, 3)
            ])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var meilleurChasseur = service.TerminerLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
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
        var partieDeChasse = UnePartieDeChasseTerminée
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.TerminerLaPartie(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
