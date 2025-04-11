using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;

namespace Bouchonnois.Tests.UseCases;

public class Tirer : UseCaseTest
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8)));

        Service.Tirer(id, Bernard);

        Repository.SavedPartieDeChasse()
            .EvenementEmis(Now, "Bernard tire")
            .ChasseurATiré(Bernard, 7);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = UnePartieDeChasseInexistante();

        var tirerQuandPartieExistePas = () => Service.Tirer(id, Bernard);

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

        var tirerSansBalle = () => Service.Tirer(id, Bernard);

        tirerSansBalle.Should().Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        Repository.SavedPartieDeChasse()
            .EvenementEmis(Now, "Bernard tire -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().EnCours());

        var chasseurInconnuVeutTirer = () => Service.Tirer(id, ChasseurInconnu);

        chasseurInconnuVeutTirer.Should().Throw<ChasseurInconnu>().WithMessage("Chasseur inconnu Chasseur inconnu");

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().ALApéro());

        var tirerEnPleinApéro = () => Service.Tirer(id, ChasseurInconnu);

        tirerEnPleinApéro.Should().Throw<OnTirePasPendantLapéroCestSacré>();

        Repository.SavedPartieDeChasse()
            .EvenementEmis(Now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée());

        var tirerQuandTerminée = () => Service.Tirer(id, ChasseurInconnu);

        tirerQuandTerminée.Should().Throw<OnTirePasQuandLaPartieEstTerminée>();

        Repository.SavedPartieDeChasse()
            .EvenementEmis(Now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }
}