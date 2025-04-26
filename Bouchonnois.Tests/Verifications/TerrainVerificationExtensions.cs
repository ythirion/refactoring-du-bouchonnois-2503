using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Verifications;

public static class TerrainVerificationExtensions
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
