using Bouchonnois.Service;

namespace Bouchonnois.Tests.Builders;

public class TerrainBuilder
{
    private const string PitibonSurSauldre = "Pitibon sur Sauldre";
    private int _nbGalinettes;

    public TerrainBuilder AyantGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public TerrainBuilder RicheEnGalinettes() => AyantGalinettes(3);

    public TerrainBuilder SansGalinettes() => AyantGalinettes(0);

    public Terrain Build() => new(PitibonSurSauldre, _nbGalinettes);
}