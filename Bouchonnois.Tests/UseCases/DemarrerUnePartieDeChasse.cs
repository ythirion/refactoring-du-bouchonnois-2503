using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using JetBrains.Annotations;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.TerrainBuilder;
using static Bouchonnois.Tests.UseCases.ArbitraryExtensions;

namespace Bouchonnois.Tests.UseCases;

using Terrain = (string nom, int nbGalinettes);
using GroupDeChasseurs = (string nom, int nbBalles)[];

public static class DesChasseursAvecDesBalles
{
    [UsedImplicitly]
    public static Arbitrary<GroupDeChasseurs> Generate() => DesChasseursAvecDesBalles();
}

public static class TerrainRicheEnGalinettes
{
    [UsedImplicitly]
    public static Arbitrary<Terrain> Generate() => TerrainRicheEnGalinettes();
}

public static class ArbitraryExtensions
{
    private static Gen<string> RandomString() => ArbMap.Default.ArbFor<string>().Generator;

    private static Arbitrary<Terrain> TerrainGenerator(int minGalinettes, int maxGalinettes)
        => (from nom in RandomString()
            from nbGalinette in Gen.Choose(minGalinettes, maxGalinettes)
            select (nom, nbGalinette)).ToArbitrary();

    public static Arbitrary<Terrain> TerrainRicheEnGalinettes() => TerrainGenerator(1, int.MaxValue);

    public static Arbitrary<Terrain> TerrainSansGalinettes() => TerrainGenerator(-int.MaxValue, 0);

    private static Arbitrary<(string nom, int nbBalles)> Chasseurs(int minBalles, int maxBalles)
        => (from nom in RandomString()
            from nbBalles in Gen.Choose(minBalles, maxBalles)
            select (nom, nbBalles)).ToArbitrary();

    private static Arbitrary<GroupDeChasseurs> GroupeDeChasseurs(int minBalles, int maxBalles)
        => (from nbChasseurs in Gen.Choose(1, 1_000)
            select Chasseurs(minBalles, maxBalles).Generator.Sample(nbChasseurs)).ToArbitrary();

    public static Arbitrary<GroupDeChasseurs> DesChasseursAvecDesBalles() => GroupeDeChasseurs(1, int.MaxValue);

    public static Arbitrary<GroupDeChasseurs> DesChasseursSansBalles() => GroupeDeChasseurs(0, 0);
}

public class DemarrerUnePartieDeChasse : UseCaseTest
{
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

        var id = DemarrerPartieUseCase.Demarrer(terrainDeChasse, chasseurs);

        Repository.SavedPartieDeChasse()
            .Should()
            .BeEquivalentTo(
                UnePartieDeChasse()
                    .IdentifiéePar(id)
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
        => (DemarrerPartieUseCase.Demarrer(terrainDeChasse, chasseurs.ToList()) == Repository.SavedPartieDeChasse().Id)
            .Label("Démarrer la partie avec succès");

    [Fact]
    public void EchoueSansChasseurs()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var demarrerPartieSansChasseurs = () => DemarrerPartieUseCase.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should().Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnTerrainSansGalinettes()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 0);

        var demarrerPartieSansChasseurs = () => DemarrerPartieUseCase.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should().Throw<ImpossibleDeDémarrerUnePartieSansGalinettes>();

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

        var demarrerPartieAvecChasseurSansBalle = () => DemarrerPartieUseCase.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should().Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
