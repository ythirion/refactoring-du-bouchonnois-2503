using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;

namespace Bouchonnois.Tests.UseCases;

public class Tirer
{
    private const string Bernard = "Bernard";
    private const string ChasseurInconnu = "Chasseur inconnu";

    private readonly DateTime _now = DateTime.Now;
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly PartieDeChasseService _service;

    public Tirer() => _service = new PartieDeChasseService(_repository, () => _now);

    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8)));

        _service.Tirer(id, Bernard);

        _repository.SavedPartieDeChasse().VerifierChasseurATiré(Bernard, 7);
        _repository.SavedPartieDeChasse().VerifierEvenementEmis(_now, "Bernard tire");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var tirerQuandPartieExistePas = () => _service.Tirer(id, Bernard);

        tirerQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }


    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(0)));

        var tirerSansBalle = () => _service.Tirer(id, Bernard);

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        _repository.SavedPartieDeChasse()
            .VerifierEvenementEmis(
                _now,
                "Bernard tire -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var chasseurInconnuVeutTirer = () => _service.Tirer(id, ChasseurInconnu);

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var tirerEnPleinApéro = () => _service.Tirer(id, ChasseurInconnu);

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        _repository.SavedPartieDeChasse()
            .VerifierEvenementEmis(
                _now,
                "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var tirerQuandTerminée = () => _service.Tirer(id, ChasseurInconnu);

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        _repository.SavedPartieDeChasse()
            .VerifierEvenementEmis(
                _now,
                "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }

    private Guid UnePartieDeChasseInexistante() => Guid.NewGuid();

    private Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        _repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }
}