using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class Tirer
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 8),
                new(nom: "Robert", ballesRestantes: 12),
            })
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);

        service.Tirer(partieDeChasse.Id, "Bernard");

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
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
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.Tirer(id, "Bernard");

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;

        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(
                new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 0),
                new(nom: "Robert", ballesRestantes: 12),
            })
            .Build();

        repository.Add(
            partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var tirerSansBalle = () => service.Tirer(partieDeChasse.Id, "Bernard");

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, "Bernard tire -> T'as plus de balles mon vieux, chasse à la main")]);
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 8),
                new(nom: "Robert", ballesRestantes: 12),
            })
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var now = DateTime.Now;

        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstALApero()
            .AvecDesChasseurs(new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 8),
                new(nom: "Robert", ballesRestantes: 12),
            })
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var tirerEnPleinApéro = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        repository
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

        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminée()
            .AvecDesChasseurs(new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 8),
                new(nom: "Robert", ballesRestantes: 12),
            })
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var tirerQuandTerminée = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")]);
    }
}
