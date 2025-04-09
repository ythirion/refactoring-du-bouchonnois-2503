using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class TirerSurUneGalinette : BaseTest
{
    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var partieDeChasse = new PartieDeChasseBuilder()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                Bernard,
                  Robert
            })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);

        service.TirerSurUneGalinette(partieDeChasse.Id, "Bernard");

        Repository.SavedPartieDeChasse()
            .HaveEmitted("Bernard tire sur une galinette")
            .ChasseurATiréSurUneGalinette(Data.Bernard, ballesRestantes: 7, galinettes: 1)
            .GalinettesRestantesSurLeTerrain(2);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.TirerSurUneGalinette(id, "Bernard");

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;


        var partieDeChasse = new PartieDeChasseBuilder()
            .AvecDesChasseurs(new List<Chasseur>
            {
                Dédé,
                new("Bernard", 0),
                  Robert
            }).Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerSansBalle = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Bernard");

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository
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
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .SansGalinettes()
            .AvecDesChasseurs(
                new List<Chasseur>
                {
                    Dédé,
                    Bernard,
                      Robert
                })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerAlorsQuePasDeGalinettes = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Bernard");

        tirerAlorsQuePasDeGalinettes.Should()
            .Throw<TasTropPicoléMonVieuxTasRienTouché>();
        Repository.SavedPartieDeChasse().Should().BeNull();
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
                  Robert
            })
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


        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstALApero()
            .AvecDesChasseurs(
                new List<Chasseur>
                {
                    Dédé,
                    Bernard,
                      Robert
                })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerEnPleinApéro = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        Repository
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


        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminée()
            .AvecDesChasseurs(
                new List<Chasseur>
                {
                    Dédé,
                    Bernard,
                      Robert
                })
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerQuandTerminée = () => service.TirerSurUneGalinette(partieDeChasse.Id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")]);
    }
}
