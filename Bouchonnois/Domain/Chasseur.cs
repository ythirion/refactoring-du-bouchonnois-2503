using Bouchonnois.Domain.Errors;

using CSharpFunctionalExtensions;

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

    public int BallesRestantes { get; private set; }

    public int NbGalinettes { get; private set; }

    public UnitResult<Error> TireDansLeVide()
        => ADesBalles()
            .Tap(() => BallesRestantes--);

    public UnitResult<Error> TireSurUneGalinette()
        => ADesBalles()
            .Tap(() =>
            {
                BallesRestantes--;
                NbGalinettes++;
            });

    private UnitResult<Error> ADesBalles()
        => BallesRestantes == 0
            ? new Error(DomainErrorMessages.TasPlusDeBallesMonVieuxChasseALaMain)
            : UnitResult.Success<Error>();

}
