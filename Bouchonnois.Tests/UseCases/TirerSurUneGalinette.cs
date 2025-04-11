using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Assertions;

using static Bouchonnois.Tests.UseCases.Data;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette : PartieDeChasseBaseTests
{
    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        service.TirerSurUneGalinette(partieDeChasse.Id, Data.Bernard);

        Repository.SavedPartieDeChasse()
            .HaveEmitted("Bernard tire sur une galinette")
            .ChasseurATiréSurUneGalinette(Data.Bernard, 7, 1)
            .GalinettesRestantesSurLeTerrain(2);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.TirerSurUneGalinette(id, Data.Bernard);

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;

        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([
                Dédé,
                new Chasseur(Data.Bernard, 0),
                Robert
            ])
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerSansBalle = () => service.TirerSurUneGalinette(partieDeChasse.Id, Data.Bernard);

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("Bernard veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main")
            .GalinettesRestantesSurLeTerrain(GalinettesSurUnTerrainRiche);
    }

    [Fact]
    public void EchoueCarPasDeGalinetteSurLeTerrain()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .AvecUnTerrainSansGalinette()
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerAlorsQuePasDeGalinettes = () => service.TirerSurUneGalinette(partieDeChasse.Id, Data.Bernard);

        tirerAlorsQuePasDeGalinettes.Should()
            .Throw<TasTropPicoléMonVieuxTasRienTouché>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var now = DateTime.Now;
        var partieDeChasse = UnePartieDeChasseALApero
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerEnPleinApéro = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!")
            .GalinettesRestantesSurLeTerrain(GalinettesSurUnTerrainRiche);
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var now = DateTime.Now;
        var partieDeChasse = UnePartieDeChasseTerminée
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(
            partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerQuandTerminée = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse()
            .HaveEmitted("Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")
            .GalinettesRestantesSurLeTerrain(GalinettesSurUnTerrainRiche);
    }
}
