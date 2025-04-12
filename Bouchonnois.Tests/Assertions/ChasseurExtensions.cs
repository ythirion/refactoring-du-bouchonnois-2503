using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Assertions;

public static class ChasseurExtensions
{
    public static Chasseur BallesRestantes(this Chasseur chasseur, int ballesRestantes) =>
        Assert(chasseur, c => c.BallesRestantes
            .Should()
            .Be(ballesRestantes,
                $"Le nombre de balles restantes pour {chasseur.Nom} devrait être de {ballesRestantes} balle(s)"
            )
        );

    public static Chasseur ATué(this Chasseur chasseur, int galinettes) =>
        Assert(chasseur,
            c => c.NbGalinettes
                .Should()
                .Be(galinettes,
                    $"Le nombre de galinettes capturées par {chasseur.Nom} devrait être de {galinettes} galinette(s)"
                )
        );

    private static Chasseur Assert(Chasseur chasseur, Action<Chasseur> assert)
    {
        assert(chasseur);
        return chasseur;
    }
}
