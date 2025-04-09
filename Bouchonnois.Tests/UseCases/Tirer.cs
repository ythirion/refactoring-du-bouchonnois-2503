using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;

namespace Bouchonnois.Tests.UseCases;

public class Tirer
{
    private const string Bernard = "Bernard";
    private const string ChasseurInconnu = "Chasseur inconnu";

    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = UnePartieDeChasse()
            .EnCours()
            .Avec(Bernard().AyantDesBalles(8))
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);

        service.Tirer(partieDeChasse.Id, Bernard);

        VerifierChasseurATiré(repository.SavedPartieDeChasse(), Bernard, 7);
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

        var partieDeChasse = UnePartieDeChasse()
            .EnCours()
            .Avec(Bernard().AyantDesBalles(0))
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);

        var tirerSansBalle = () => service.Tirer(partieDeChasse.Id, Bernard);

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();
        
        VerifierEvenementEmis(
            repository.SavedPartieDeChasse(),
            now,
            "Bernard tire -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = UnePartieDeChasse()
            .EnCours()
            .Avec(Bernard())
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);

        var chasseurInconnuVeutTirer = () => service.Tirer(partieDeChasse.Id, ChasseurInconnu);

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

        var partieDeChasse = UnePartieDeChasse()
            .EnApéro()
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);

        var tirerEnPleinApéro = () => service.Tirer(partieDeChasse.Id, ChasseurInconnu);

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        VerifierEvenementEmis(
            repository.SavedPartieDeChasse(),
            now,
            "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var now = DateTime.Now;

        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = UnePartieDeChasse()
            .Terminée()
            .Build();

        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);

        var tirerQuandTerminée = () => service.Tirer(partieDeChasse.Id, ChasseurInconnu);

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        VerifierEvenementEmis(
            repository.SavedPartieDeChasse(),
            now,
            "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }

    private static void VerifierEvenementEmis(PartieDeChasse partieDeChasse, DateTime now, string message)
        => partieDeChasse
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, message)]);

    private static void VerifierChasseurATiré(PartieDeChasse savedPartieDeChasse, string nom, int ballesRestantes)
        => savedPartieDeChasse.Chasseurs.First(c => c.Nom == nom).BallesRestantes.Should().Be(ballesRestantes);
}