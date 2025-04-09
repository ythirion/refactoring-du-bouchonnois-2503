using Bouchonnois.Domain;
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
        var partieDeChasse = PartieDeChasseExistante(
            _ =>
                _.EnCours()
                    .Avec(Bernard().AyantDesBalles(8)));

        _service.Tirer(partieDeChasse.Id, Bernard);

        _repository.SavedPartieDeChasse().VerifierChasseurATiré(Bernard, 7);
    }
    
    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();

        var tirerQuandPartieExistePas = () => _service.Tirer(id, Bernard);

        tirerQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var partieDeChasse = PartieDeChasseExistante(
            _ =>
                _.EnCours()
                    .Avec(Bernard().AyantDesBalles(0)));

        var tirerSansBalle = () => _service.Tirer(partieDeChasse.Id, Bernard);

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
        var partieDeChasse = PartieDeChasseExistante(_ => _.EnCours().Avec(Bernard()));

        var chasseurInconnuVeutTirer = () => _service.Tirer(partieDeChasse.Id, ChasseurInconnu);

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Chasseur inconnu");

        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var partieDeChasse = PartieDeChasseExistante(_ => _.EnApéro());

        var tirerEnPleinApéro = () => _service.Tirer(partieDeChasse.Id, ChasseurInconnu);

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
        var partieDeChasse = PartieDeChasseExistante(_ => _.Terminée());

        var tirerQuandTerminée = () => _service.Tirer(partieDeChasse.Id, ChasseurInconnu);

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        _repository.SavedPartieDeChasse()
            .VerifierEvenementEmis(
                _now,
                "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }

    private PartieDeChasse PartieDeChasseExistante(Func<PartieDeChasseBuilder, PartieDeChasseBuilder> setup)
    {
        var partieDeChasse = setup(UnePartieDeChasse()).Build();

        _repository.Add(partieDeChasse);

        return partieDeChasse;
    }
}