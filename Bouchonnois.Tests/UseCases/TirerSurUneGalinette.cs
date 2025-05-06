using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.UseCases.TirerSurGalinette;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette : UseCaseTest
{
    private readonly UseCase _tirerSurGalinette;
    public TirerSurUneGalinette()
    {
        _tirerSurGalinette = new UseCase(
            Repository,
            () => Now
        );
    }

    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8).Brocouille())
                .SurUnTerrainAyantGalinettes(3));

        _tirerSurGalinette.Handle(new Request(id, Bernard));

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard tire sur une galinette")
            .TerrainDevraitAvoirGalinettesRestantes(2)
            .ChasseurDevraitAvoirTiréSurUneGalinette(Bernard, 7, 1);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var tirerQuandPartieExistePas = () => _tirerSurGalinette.Handle(new Request(id, "Bernard"));

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

        var tirerSansBalle = () => _tirerSurGalinette.Handle(new Request(id, Bernard));

        tirerSansBalle.Should().Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(
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

        var tirerAlorsQuePasDeGalinettes = () => _tirerSurGalinette.Handle(new Request(id, Bernard));

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

        var chasseurInconnuVeutTirer = () => _tirerSurGalinette.Handle(new Request(id, ChasseurInconnu));

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

        var tirerEnPleinApéro = () => _tirerSurGalinette.Handle(new Request(id, ChasseurInconnu));

        tirerEnPleinApéro.Should().Throw<OnTirePasPendantLapéroCestSacré>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée().SurUnTerrainRicheEnGalinettes());

        var tirerQuandTerminée = () => _tirerSurGalinette.Handle(new Request(id, "Chasseur inconnu"));

        tirerQuandTerminée.Should().Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }
}
