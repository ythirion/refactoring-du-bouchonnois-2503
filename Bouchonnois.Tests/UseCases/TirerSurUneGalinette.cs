using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette
{
    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
                status: PartieStatus.EnCours,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        service.TirerSurUneGalinette(id, "Bernard");

        repository.SavedPartieDeChasse()
            .HaveEmitted("Bernard tire sur une galinette")
            .ChasseurATiréSurUneGalinette(Data.Bernard, ballesRestantes: 7, galinettes: 1)
            .GalinettesRestantesSurLeTerrain(2);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.TirerSurUneGalinette(id, "Bernard");

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;

        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        PartieDeChasse partieDeChasse = new PartieDeChasse(
            id: id,
            chasseurs: new List<Chasseur>
            {
                new(nom: "Dédé", ballesRestantes: 20),
                new(nom: "Bernard", ballesRestantes: 0),
                new(nom: "Robert", ballesRestantes: 12),
            },
            terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
            status: PartieStatus.EnCours,
            events: new List<Event>());

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        var tirerSansBalle = () => service.TirerSurUneGalinette(id, "Bernard");

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
            [
                new Event(
                    now,
                    "Bernard veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main")
            ]);
    }

    [Fact]
    public void EchoueCarPasDeGalinetteSurLeTerrain()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 0),
                status: PartieStatus.EnCours));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var tirerAlorsQuePasDeGalinettes = () => service.TirerSurUneGalinette(id, "Bernard");

        tirerAlorsQuePasDeGalinettes.Should()
            .Throw<TasTropPicoléMonVieuxTasRienTouché>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
                status: PartieStatus.EnCours));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.TirerSurUneGalinette(id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var now = DateTime.Now;

        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
                status: PartieStatus.Apéro,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => now);
        var tirerEnPleinApéro = () => service.TirerSurUneGalinette(id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
            [
                new Event(now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!")
            ]);
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var now = DateTime.Now;

        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
                status: PartieStatus.Terminée,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => now);
        var tirerQuandTerminée = () => service.TirerSurUneGalinette(id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        repository.SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")]);
    }
}
