using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.TerrainBuilder;
using static Bouchonnois.Tests.UseCases.ArbitraryExtensions;

namespace Bouchonnois.Tests.UseCases;

public static class ArbitraryExtensions
{
    private static Gen<string> RandomString() => ArbMap.Default.ArbFor<string>().Generator;

    private static Arbitrary<(string nom, int nbGalinettes)> TerrainGenerator(int minGalinettes, int maxGalinettes)
        => (from nom in RandomString()
            from nbGalinette in Gen.Choose(minGalinettes, maxGalinettes)
            select (nom, nbGalinette)).ToArbitrary();

    public static Arbitrary<(string nom, int nbGalinettes)> TerrainRicheEnGalinettes()
        => TerrainGenerator(1, int.MaxValue);

    public static Arbitrary<(string nom, int nbGalinettes)> TerrainSansGalinettes()
        => TerrainGenerator(-int.MaxValue, 0);

    private static Arbitrary<(string nom, int nbBalles)> Chasseurs(int minBalles, int maxBalles)
        => (from nom in RandomString()
            from nbBalles in Gen.Choose(minBalles, maxBalles)
            select (nom, nbBalles)).ToArbitrary();

    private static Arbitrary<(string nom, int nbBalles)[]> GroupeDeChasseurs(int minBalles, int maxBalles)
        => (from nbChasseurs in Gen.Choose(1, 1_000)
            select Chasseurs(minBalles, maxBalles).Generator.Sample(nbChasseurs)).ToArbitrary();

    public static Arbitrary<(string nom, int nbBalles)[]> DesChasseursAvecDesBalles()
        => GroupeDeChasseurs(1, int.MaxValue);

    public static Arbitrary<(string nom, int nbBalles)[]> DesChasseursSansBalles() => GroupeDeChasseurs(0, 0);
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

        var id = Service.Demarrer(terrainDeChasse, chasseurs);

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
    public Property DémarrerAvecSucces()
        => Prop.ForAll(
            DesChasseursAvecDesBalles(),
            TerrainRicheEnGalinettes(),
            (chasseurs, terrainDeChasse) =>
            {
                Service.Demarrer(terrainDeChasse, chasseurs.ToList());


                return Repository.SavedPartieDeChasse() is not null;
            });

    [Fact]
    public void EchoueSansChasseurs()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var demarrerPartieSansChasseurs = () => Service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should().Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnTerrainSansGalinettes()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 0);

        var demarrerPartieSansChasseurs = () => Service.Demarrer(terrainDeChasse, chasseurs);

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

        var demarrerPartieAvecChasseurSansBalle = () => Service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should().Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}