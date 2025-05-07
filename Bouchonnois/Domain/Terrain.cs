using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class Terrain
{
    public Terrain(string nom, int nbGalinettes)
    {
        Nom = nom;
        NbGalinettes = nbGalinettes;
    }

    public string Nom { get; init; }

    public int NbGalinettes { get; set; }

    public Result<Terrain, Error> ChasseurTueUneGalinette()
    {
        if (SansGalinettes()) return Errors.TasTropPicoléMonVieuxTasRienTouché();

        NbGalinettes--;

        return this;
    }

    private bool SansGalinettes() => NbGalinettes == 0;
}