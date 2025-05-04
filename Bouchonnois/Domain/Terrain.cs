using Bouchonnois.Domain.Errors;

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
            return new Error(DomainErrorMessages.IlNyAPlusDeGalinettesSurCeTerrain);
        }

        NbGalinettes--;
        return UnitResult.Success<Error>();
    }
}
