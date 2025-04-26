using FsCheck;

using JetBrains.Annotations;

using Terrain = (string nom, int nbGalinettes);

namespace Bouchonnois.Tests.UseCases.Common;

public static class TerrainRicheEnGalinettes
{
    [UsedImplicitly]
    public static Arbitrary<Terrain> Generate() => ArbitraryExtensions.TerrainRicheEnGalinettes();
}
