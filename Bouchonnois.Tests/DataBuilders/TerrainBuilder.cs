using Bouchonnois.Service;

namespace Bouchonnois.Tests.DataBuilders;

public class TerrainBuilder
{
    private string _nom = "Pitibon sur Sauldre";
    private int _nbGalinettes;

    public TerrainBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }

    public TerrainBuilder AvecGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public Terrain Build()
    {
        return new Terrain(
            nom: _nom,
            nbGalinettes: _nbGalinettes
        );
    }
}
