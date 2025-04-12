using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette : UseCaseTest
{
    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8).SansGalinettes())
                .SurUnTerrainAyantGalinettes(3));

        Service.TirerSurUneGalinette(id, Bernard);

        Repository.SavedPartieDeChasse()
            .AEmisEvenement(Now, "Bernard tire sur une galinette")
            .GalinettesRestantesSurLeTerrain(2)
            .ChasseurATiréSurUneGalinette(Bernard, ballesRestantes: 7, galinettesCapturées: 1);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var tirerQuandPartieExistePas = () => Service.TirerSurUneGalinette(id, "Bernard");

        tirerQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().SansBalles())
                .SurUnTerrainAyantGalinettes(3));

        var tirerSansBalle = () => Service.TirerSurUneGalinette(id, Bernard);

        tirerSansBalle.Should().Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository
            .SavedPartieDeChasse()
            .AEmisEvenement(
                Now,
                "Bernard veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarPasDeGalinetteSurLeTerrain()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard())
                .SurUnTerrainSansGalinettes());

        var tirerAlorsQuePasDeGalinettes = () => Service.TirerSurUneGalinette(id, Bernard);

        tirerAlorsQuePasDeGalinettes.Should().Throw<TasTropPicoléMonVieuxTasRienTouché>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .SurUnTerrainRicheEnGalinettes());

        var chasseurInconnuVeutTirer = () => Service.TirerSurUneGalinette(id, ChasseurInconnu);

        chasseurInconnuVeutTirer.Should().Throw<ChasseurInconnu>().WithMessage("Chasseur inconnu Chasseur inconnu");

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro()
                .SurUnTerrainRicheEnGalinettes());

        var tirerEnPleinApéro = () => Service.TirerSurUneGalinette(id, ChasseurInconnu);

        tirerEnPleinApéro.Should().Throw<OnTirePasPendantLapéroCestSacré>();

        Repository
            .SavedPartieDeChasse()
            .AEmisEvenement(Now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
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
                    new(nom: "Robert", ballesRestantes: 12)
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
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