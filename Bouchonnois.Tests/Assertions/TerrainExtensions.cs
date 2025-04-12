using Bouchonnois.Service;

namespace Bouchonnois.Tests.Assertions;

public static class TerrainExtensions
{
    public static Terrain GalinettesRestantes(this Terrain terrain, int nbGalinettes)
    {
        terrain
            .NbGalinettes
            .Should()
            .Be(nbGalinettes, $"Le terrain devrait contenir {nbGalinettes} mais en contient {terrain.NbGalinettes}");

        return terrain;
    }
}
