using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.Tests.Builders;

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
            .FailWith(new Error(DomainErrorMessages.IlNyAPlusDeGalinettesSurCeTerrain));
    }
}
