using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Verifications;

public static class ChasseurVerificationExtensions
{
    public static Chasseur BallesRestantes(this Chasseur chasseur, int ballesRestantes)
        => Assert(
            chasseur,
            c => c.BallesRestantes
                .Should()
                .Be(
                    ballesRestantes,
                    $"Le nombre de balles restantes pour {chasseur.Nom} devrait être de {ballesRestantes} balle(s)"));

    public static Chasseur ATué(this Chasseur chasseur, int galinettesCapturées)
        => Assert(
            chasseur,
            c => c.NbGalinettes
                .Should()
                .Be(
                    galinettesCapturées,
                    $"Le nombre de galinettes capturées par {chasseur.Nom} devrait être de {galinettesCapturées} galinette(s)"));

    private static Chasseur Assert(Chasseur chasseur, Action<Chasseur> assert)
    {
        assert(chasseur);
        return chasseur;
    }
}