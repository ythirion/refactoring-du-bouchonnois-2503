using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Exceptions;

using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class Tirer : UseCaseTest
{
    private readonly TirerUseCase _tirerUseCase;
    public Tirer()
    {
        _tirerUseCase = new TirerUseCase(
            Repository,
            () => Now
        );
    }

    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8)));

        _tirerUseCase.Handle(id, Bernard);

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard tire")
            .ChasseurDevraitAvoirTiré(Bernard, 7);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var tirerQuandPartieExistePas = () => _tirerUseCase.Handle(id, Bernard);

        tirerQuandPartieExistePas.Should().Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().SansBalles()));

        var tirerSansBalle = () => _tirerUseCase.Handle(id, Bernard);

        tirerSansBalle.Should().Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard tire -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var chasseurInconnuVeutTirer = () => _tirerUseCase.Handle(id, ChasseurInconnu);

        chasseurInconnuVeutTirer.Should().Throw<ChasseurInconnu>().WithMessage("Chasseur inconnu Chasseur inconnu");

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var tirerEnPleinApéro = () => _tirerUseCase.Handle(id, ChasseurInconnu);

        tirerEnPleinApéro.Should().Throw<OnTirePasPendantLapéroCestSacré>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var tirerQuandTerminée = () => _tirerUseCase.Handle(id, ChasseurInconnu);

        tirerQuandTerminée.Should().Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }
}
