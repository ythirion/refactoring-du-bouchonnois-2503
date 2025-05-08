using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class Terrain
{
    public Terrain(string nom, int nbGalinettes)
    {
        Nom = nom;
        NbGalinettes = nbGalinettes;
    }

    public string Nom { get; }

    public int NbGalinettes { get; private set; }

    public UnitResult<Error> UneGalinetteEnMoins()
    {
        if (NbGalinettes <= 0)
        {
            return Error.IlNyAPlusDeGalinettesSurCeTerrainError();
        }

        NbGalinettes--;
        return UnitResultExtensions.Success();
    }
}
