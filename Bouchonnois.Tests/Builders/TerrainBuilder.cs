using Bouchonnois.Service;

namespace Bouchonnois.Tests.Builders;

public class TerrainBuilder
{
    private int _nbGalinettes;
    private string _nom = "Pitibon sur Sauldre";

    public static TerrainBuilder UnTerrain() => new();
    
    public TerrainBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }
    
    public TerrainBuilder AyantGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public TerrainBuilder RicheEnGalinettes() => AyantGalinettes(3);

    public TerrainBuilder SansGalinettes() => AyantGalinettes(0);

    public Terrain Build() => new(_nom, _nbGalinettes);
}