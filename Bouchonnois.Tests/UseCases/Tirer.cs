using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

using static Bouchonnois.Tests.UseCases.Data;

namespace Bouchonnois.Tests.UseCases;

public class Tirer
{
    private readonly PartieDeChasseRepositoryForTests _repository;
    private readonly PartieDeChasseService _service;

    public Tirer()
    {
        _repository = new PartieDeChasseRepositoryForTests();
        _service = new PartieDeChasseService(_repository, () => DateTime.Now);
    }

    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var partieDeChasse =
            new PartieDeChasseBuilder()
            .AjouterChasseur(new(Dede, 20))
            .AjouterChasseur(new(Bernard, 8))
            .AjouterChasseur(new(Robert, 12))
            .Build();
        _repository.Add(partieDeChasse);

        _service.Tirer(partieDeChasse.Id, Bernard);

        _repository
            .SavedPartieDeChasse()
            .HaveEmitted($"{Bernard} tire")
            .ChasseurATiré(Bernard, ballesRestantes: 7)
            .GalinettesRestantesSurLeTerrain(GalinettesSurUnTerrainRiche);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var tirerQuandPartieExistePas = () => _service.Tirer(Guid.Empty, Bernard);

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var partieDeChasse =
            new PartieDeChasseBuilder()
                .AjouterChasseur(new(Dede, 20))
                .AjouterChasseur(new(Bernard, 0))
                .AjouterChasseur(new(Robert, 12))
                .Build();
        _repository.Add(partieDeChasse);

        var now = DateTime.Now;
        var service = new PartieDeChasseService(_repository, () => now);
        var tirerSansBalle = () => service.Tirer(partieDeChasse.Id, Bernard);

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        _repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, $"{Bernard} tire -> T'as plus de balles mon vieux, chasse à la main")]);
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var partieDeChasse =
            new PartieDeChasseBuilder()
                .AjouterChasseur(new(Dede, 20))
                .AjouterChasseur(new(Bernard, 0))
                .AjouterChasseur(new(Robert, 12))
                .Build();
        _repository.Add(partieDeChasse);

        var chasseurInconnuVeutTirer = () => _service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var now = DateTime.Now;

        var partieDeChasse =
            new PartieDeChasseBuilder()
                .QuandLesChasseursSontALapero()
                .AjouterChasseur(new(Dede, 20))
                .AjouterChasseur(new(Bernard, 8))
                .AjouterChasseur(new(Robert, 12))
                .Build();
        _repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(_repository, () => now);
        var tirerEnPleinApéro = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        _repository
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
        var partieDeChasse =
            new PartieDeChasseBuilder()
                .QuandLaPartieDeChasseEstTerminee()
                .AjouterChasseur(new(Dede, 20))
                .AjouterChasseur(new(Bernard, 8))
                .AjouterChasseur(new(Robert, 12))
                .Build();
        _repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(_repository, () => now);
        var tirerQuandTerminée = () => service.Tirer(partieDeChasse.Id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        _repository
            .SavedPartieDeChasse()
            .Events
            .Should()
            .BeEquivalentTo(
                [new Event(now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée")]);
    }
}
