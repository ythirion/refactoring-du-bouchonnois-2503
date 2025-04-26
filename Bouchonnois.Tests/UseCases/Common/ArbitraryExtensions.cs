using FsCheck;
using FsCheck.Fluent;

using GroupDeChasseurs = (string nom, int nbBalles)[];
using Terrain = (string nom, int nbGalinettes);

namespace Bouchonnois.Tests.UseCases.Common;

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
