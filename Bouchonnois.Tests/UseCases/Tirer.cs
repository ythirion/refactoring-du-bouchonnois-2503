using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.UseCases.ChasseurBuilder;

namespace Bouchonnois.Tests.UseCases;

public class Tirer
{
    private const string Bernard = "Bernard";

    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = UnePartieDeChasse()
            .EnCours()
            .Avec(Bernard().AyantDesBalles(8).Build())
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);

        service.Tirer(partieDeChasse.Id, Bernard);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        VerifierChasseurATiré(savedPartieDeChasse, Bernard, 7);
    }

    private static void VerifierChasseurATiré(PartieDeChasse savedPartieDeChasse, string nom, int ballesRestantes)
    {
        savedPartieDeChasse.Chasseurs.First(c => c.Nom == nom).BallesRestantes.Should().Be(ballesRestantes);
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

        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 0),
                    new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.EnCours,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => now);
        var tirerSansBalle = () => service.Tirer(id, "Bernard");

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
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.EnCours));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.Tirer(id, "Chasseur inconnu");

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
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Apéro,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => now);
        var tirerEnPleinApéro = () => service.Tirer(id, "Chasseur inconnu");

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
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Terminée,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => now);
        var tirerQuandTerminée = () => service.Tirer(id, "Chasseur inconnu");

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

public class ChasseurBuilder
{
    private string _nom = "Chasseur Inconnu";
    private int _ballesRestantes = 0;
    
    public static ChasseurBuilder Bernard() => new ChasseurBuilder().Nommé("Bernard").AyantDesBalles(8);

    public ChasseurBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }
    
    public ChasseurBuilder AyantDesBalles(int ballesRestantes)
    {
        _ballesRestantes = ballesRestantes;
        return this;
    }

    public Chasseur Build() => new(_nom, _ballesRestantes);
}