using Bouchonnois.Domain.Common;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.UseCases.TirerSurGalinette;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette : UseCaseTest
{
    private readonly UseCase _tirerSurGalinette;

    public TirerSurUneGalinette()
        => _tirerSurGalinette = new UseCase(
            Repository,
            () => Now);

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

        _tirerSurGalinette.Handle(new Request(id, "Bernard"))
            .Should()
            .FailWith(Errors.LaPartieDeChasseNexistePas());

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

        _tirerSurGalinette.Handle(new Request(id, Bernard))
            .Should()
            .FailWith(Errors.TasPlusDeBallesMonVieuxChasseALaMain());

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

        _tirerSurGalinette.Handle(new Request(id, Bernard))
            .Should()
            .FailWith(Errors.TasTropPicoléMonVieuxTasRienTouché());

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .SurUnTerrainRicheEnGalinettes());

        _tirerSurGalinette.Handle(new Request(id, ChasseurInconnu))
            .Should()
            .FailWith(Errors.ChasseurInconnu(ChasseurInconnu));

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .ALApéro()
                .Avec(Bernard())
                .SurUnTerrainRicheEnGalinettes());

        _tirerSurGalinette.Handle(new Request(id, Bernard))
            .Should()
            .FailWith(Errors.OnTirePasPendantLapéroCestSacré());

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .Terminée()
                .Avec(Bernard())
                .SurUnTerrainRicheEnGalinettes());

        _tirerSurGalinette.Handle(new Request(id, Bernard))
            .Should()
            .FailWith(Errors.OnTirePasQuandLaPartieEstTerminée());

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard veut tirer -> On tire pas quand la partie est terminée");
    }
}