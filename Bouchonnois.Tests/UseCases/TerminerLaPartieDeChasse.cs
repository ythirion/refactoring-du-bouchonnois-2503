using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Assertions;

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

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .BeTerminé();

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

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .BeTerminé();

        meilleurChasseur.Should().Be(Data.Robert);
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

        Repository.SavedPartieDeChasse()
            .HaveEmitted("La partie de chasse est terminée, vainqueur : Dédé - 2 galinettes, Bernard - 2 galinettes")
            .BeTerminé();
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

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("La partie de chasse est terminée, vainqueur : Brocouille")
            .BeTerminé();

        meilleurChasseur.Should().Be("Brocouille");
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

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes")
            .BeTerminé();

        meilleurChasseur.Should().Be("Dédé, Bernard, Robert");
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
