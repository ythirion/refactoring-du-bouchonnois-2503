using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.Tests.Verifications;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Errors;

using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases.Commands;

public class TirerSurUneGalinette : UseCaseTest
{
    private readonly TirerSurGalinetteUseCase _tirerSurGalinetteUseCase;
    public TirerSurUneGalinette()
    {
        _tirerSurGalinetteUseCase = new TirerSurGalinetteUseCase(
            Repository,
            () => Now
        );
    }

    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours()
                .Avec(Bernard().AyantDesBalles(8).Brocouille())
                .SurUnTerrainAyantGalinettes(3));

        _tirerSurGalinetteUseCase
            .Handle(id, Bernard)
            .Should()
            .Succeed();

        Repository.SavedPartieDeChasse()
            .DevraitAvoirEmis(Now, "Bernard tire sur une galinette")
            .TerrainDevraitAvoirGalinettesRestantes(2)
            .ChasseurDevraitAvoirTiréSurUneGalinette(Bernard, 7, 1);
    }

    public class Failure : UseCaseTest
    {
        private readonly TirerSurGalinetteUseCase _sut;
        public Failure()
        {
            _sut = new TirerSurGalinetteUseCase(
                Repository,
                () => Now
            );
        }


        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = UnePartieDeChasseInexistante();

            _sut
                .Handle(id, "Bernard")
                .Should()
                .FailWith(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas));

            Repository.SavedPartieDeChasse()
                .Should()
                .BeNull();
        }

        [Fact]
        public void EchoueAvecUnChasseurNayantPlusDeBalles()
        {
            var id = UnePartieDeChasseExistante(
                UnePartieDeChasse()
                    .EnCours()
                    .Avec(Bernard().SansBalles())
                    .SurUnTerrainAyantGalinettes(3));

            _sut
                .Handle(id, Bernard)
                .Should()
                .FailWith(new Error(DomainErrorMessages.TasPlusDeBallesMonVieuxChasseALaMain));

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

            _sut
                .Handle(id, Bernard)
                .Should()
                .FailWith(new Error(DomainErrorMessages.TasTropPicoléMonVieuxTasRienTouché));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueCarLeChasseurNestPasDansLaPartie()
        {
            var id = UnePartieDeChasseExistante(
                UnePartieDeChasse()
                    .EnCours()
                    .SurUnTerrainRicheEnGalinettes());

            _sut
                .Handle(id, ChasseurInconnu)
                .Should()
                .FailWith(new Error(DomainErrorMessages.LeChasseurNestPasDansLaPartie));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLesChasseursSontEnApero()
        {
            var id = UnePartieDeChasseExistante(
                UnePartieDeChasse()
                    .ALApéro()
                    .SurUnTerrainRicheEnGalinettes());

            _sut
                .Handle(id, ChasseurInconnu)
                .Should()
                .FailWith(new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré));

            Repository.SavedPartieDeChasse()
                .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
        }

        [Fact]
        public void EchoueSiLaPartieDeChasseEstTerminée()
        {
            var id = UnePartieDeChasseExistante(UnePartieDeChasse().Terminée().SurUnTerrainRicheEnGalinettes());

            _sut
                .Handle(id, "Chasseur inconnu")
                .Should()
                .FailWith(new Error(DomainErrorMessages.OnTirePasQuandLaPartieEstTerminée));

            Repository.SavedPartieDeChasse()
                .DevraitAvoirEmis(Now, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
        }
    }
}
