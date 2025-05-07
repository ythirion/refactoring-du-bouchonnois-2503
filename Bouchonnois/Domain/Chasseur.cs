using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;
using static Bouchonnois.Domain.Common.Errors;

namespace Bouchonnois.Domain;

public class Chasseur
{
    public Chasseur(string nom, int ballesRestantes, int nbGalinettes = 0)
    {
        Nom = nom;
        BallesRestantes = ballesRestantes;
        NbGalinettes = nbGalinettes;
    }

    public string Nom { get; }

    public int BallesRestantes { get; set; }

    public int NbGalinettes { get; set; }

    public Result<Chasseur, Error> Tire()
    {
        if (NaPlusDeBalles()) return TasPlusDeBallesMonVieuxChasseALaMain();

        BallesRestantes--;
        NbGalinettes++;

        return this;
    }

    private bool NaPlusDeBalles() => BallesRestantes == 0;
}
