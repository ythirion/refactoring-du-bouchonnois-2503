using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Builders;

namespace Bouchonnois.Tests.UseCases;

public class Tirer : BaseTest
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);

        service.Tirer(partieDeChasse.Id, "Bernard");

        var savedPartieDeChasse = Repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(partieDeChasse.Id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.EnCours);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(7);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.Tirer(id, "Bernard");

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(
                new List<Chasseur>
                {
                    Dédé,
                    new("Bernard", 0),
                    new("Robert", 12)
                })
            .Build();

        Repository.Add(
            partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerSansBalle = () => service.Tirer(partieDeChasse.Id, "Bernard");

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, "Bernard tire -> T'as plus de balles mon vieux, chasse à la main")]);
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var now = DateTime.Now;


        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstALApero()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerEnPleinApéro = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        Repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!")]);
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var now = DateTime.Now;


        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminée()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                new("Robert", 12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerQuandTerminée = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")]);
    }
}
