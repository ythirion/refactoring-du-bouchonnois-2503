using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.Tests.UseCases.Common;
using Bouchonnois.UseCases.Commands;

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.TerrainBuilder;
using static Bouchonnois.Tests.UseCases.Common.ArbitraryExtensions;

namespace Bouchonnois.Tests.UseCases.Commands;

using Terrain = (string nom, int nbGalinettes);
using GroupDeChasseurs = (string nom, int nbBalles)[];

public class DemarrerUnePartieDeChasse : UseCaseTest
{
    private readonly DemarrerUseCase _sut;
    public DemarrerUnePartieDeChasse()
    {
        _sut = new DemarrerUseCase(Repository, () => Now);
        ;
    }

    [Fact]
    public void AvecPlusieursChasseurs()
    {
        var chasseurs = new List<(string, int)>
        {
            (Dédé, 20),
            (Bernard, 8),
            (Robert, 12)
        };

        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var guidOrError
            = _sut.Handle(terrainDeChasse, chasseurs);
        guidOrError
            .Should()
            .Succeed();

        Repository.SavedPartieDeChasse()
            .Should()
            .BeEquivalentTo(
                UnePartieDeChasse()
                    .IdentifiéePar(guidOrError.Value)
                    .EnCours()
                    .Sur(UnTerrain().Nommé("Pitibon sur Sauldre").AyantGalinettes(3))
                    .Avec(
                        Dédé().AyantDesBalles(20),
                        Bernard().AyantDesBalles(8),
                        Robert().AyantDesBalles(12))
                    .AvecEvenements(
                        new Event(
                            Now,
                            "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"))
                    .Build());
    }

    [Property]
    public Property Sur1TerrainRicheEnGalinettesEtAvecDesChasseurAvecDesBalles()
        => Prop.ForAll(
            TerrainRicheEnGalinettes(),
            DesChasseursAvecDesBalles(),
            DémarrerLaPartieAvecSuccès);

    [Property(Arbitrary = [typeof(TerrainRicheEnGalinettes), typeof(DesChasseursAvecDesBalles)])]
    public Property Sur1TerrainRicheEnGalinettesEtAvecDesChasseurAvecDesBalles_2(
        Terrain terrainRicheEnGalinettes,
        GroupDeChasseurs chasseursAvecDesBalles)
        => DémarrerLaPartieAvecSuccès(terrainRicheEnGalinettes, chasseursAvecDesBalles);

    private Property DémarrerLaPartieAvecSuccès(Terrain terrainDeChasse, GroupDeChasseurs chasseurs)
        => (_sut.Handle(terrainDeChasse, chasseurs.ToList()).Value == Repository.SavedPartieDeChasse().Id)
            .Label("Démarrer la partie avec succès");

    public class Failure : UseCaseTest
    {
        private readonly DemarrerUseCase _sut;
        public Failure()
        {
            _sut = new DemarrerUseCase(Repository, () => Now);
        }

        [Fact]
        public void EchoueSansChasseurs()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = ("Pitibon sur Sauldre", 3);

            _sut.Handle(terrainDeChasse, chasseurs)
                .Should()
                .FailWith(new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansChasseur));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueAvecUnTerrainSansGalinettes()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = ("Pitibon sur Sauldre", 0);

            _sut.Handle(terrainDeChasse, chasseurs)
                .Should()
                .FailWith(new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansGalinettes));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiChasseurSansBalle()
        {
            var chasseurs = new List<(string, int)>
            {
                (Dédé, 20),
                (Bernard, 0)
            };

            var terrainDeChasse = ("Pitibon sur Sauldre", 3);

            _sut
                .Handle(terrainDeChasse, chasseurs)
                .Should()
                .FailWith(new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle));

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}
