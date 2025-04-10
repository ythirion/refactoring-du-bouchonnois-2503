using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.Assertions;

using static Bouchonnois.Tests.UseCases.Data;

namespace Bouchonnois.Tests.UseCases;

public class Tirer : PartieDeChasseBaseTests
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        service.Tirer(partieDeChasse.Id, Data.Bernard);

        Repository
            .SavedPartieDeChasse()
            .HaveEmitted("Bernard tire")
            .ChasseurATiré(Data.Bernard, 7)
            .GalinettesRestantesSurLeTerrain(GalinettesSurUnTerrainRiche);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.Tirer(id, Data.Bernard);

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var now = DateTime.Now;

        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, new Chasseur(Data.Bernard, 0), Robert])
            .Build();

        Repository.Add(partieDeChasse);

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
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();

        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.Tirer(partieDeChasse.Id, Data.ChasseurInconnu);

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
        var tirerEnPleinApéro = () => service.Tirer(partieDeChasse.Id, Data.ChasseurInconnu);

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

        var partieDeChasse = UnePartieDeChasseTerminée
            .AvecDesChasseurs([Dédé, Bernard, Robert])
            .Build();
        Repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(Repository, () => now);
        var tirerQuandTerminée = () => service.Tirer(partieDeChasse.Id, Data.ChasseurInconnu);

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
