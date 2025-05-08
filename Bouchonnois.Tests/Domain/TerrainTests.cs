using Bouchonnois.Domain;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Verifications;

namespace Bouchonnois.Tests.Domain;

public class TerrainTests
{
    [Fact]
    public void EnleverUnGalinetteAuTerrainQuiADesGalinettes_ShouldSucceed()
    {
        var terrain = new TerrainBuilder()
            .RicheEnGalinettes()
            .Build();

        terrain.UneGalinetteEnMoins()
            .Should()
            .Succeed();
    }

    [Fact]
    public void EnleverUnGalinetteAuTerrainSansGalinettes_ShouldFail()
    {
        var terrain = new TerrainBuilder()
            .SansGalinettes()
            .Build();

        terrain.UneGalinetteEnMoins()
            .Should()
            .FailWith(Error.IlNyAPlusDeGalinettesSurCeTerrainError())
            .ExpectMessageToBe("Il n'y a plus de galinettes sur ce terrain");
    }
}
