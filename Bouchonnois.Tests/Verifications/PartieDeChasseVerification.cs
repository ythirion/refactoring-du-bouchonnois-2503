using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.Verifications;

public static class Verification
{
    public static PartieDeChasse DoitAvoirEmis(
        this PartieDeChasse partieDeChasse,
        DateTime date,
        string message)
        => Assert(
            partieDeChasse,
            p =>
                p.Events
                    .Should()
                    .HaveCount(1)
                    .And
                    .BeEquivalentTo(
                        [
                            new Event(date, message)
                        ],
                        $"Les events devraient contenir {message}."));


    private static Chasseur Chasseur(this PartieDeChasse partieDeChasse, string nom)
        => partieDeChasse
            .Chasseurs
            .Should()
            .ContainSingle(c => c.Nom == nom, "Chasseur non présent dans la partie de chasse")
            .Subject;

    public static PartieDeChasse ChasseurDoitAvoirTiréSurUneGalinette(
        this PartieDeChasse partieDeChasse,
        string nom,
        int ballesRestantes,
        int galinettesCapturées)
        => Assert(
            partieDeChasse,
            p =>
                p.Chasseur(nom)
                    .BallesRestantes(ballesRestantes)
                    .ATué(galinettesCapturées));


    public static PartieDeChasse ChasseurATiré(
        this PartieDeChasse partieDeChasse,
        string nom,
        int ballesRestantes)
        => Assert(
            partieDeChasse,
            p =>
                p.Chasseur(nom)
                    .BallesRestantes(ballesRestantes));

    public static PartieDeChasse GalinettesRestantesSurLeTerrain(
        this PartieDeChasse partieDeChasse,
        int nbGalinettes)
        => Assert(
            partieDeChasse,
            p =>
                p.Terrain.GalinettesRestantes(nbGalinettes));

    public static PartieDeChasse EstALApéro(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.Apéro, "Les chasseurs devraient être à l'apéro"));

    public static PartieDeChasse EstEnCours(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.EnCours, "Les chasseurs devraient être en cours de chasse"));

    public static PartieDeChasse EstTerminé(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.Terminée, "La partie de Chasse devrait être terminée"));

    private static PartieDeChasse Assert(PartieDeChasse partieDeChasse, Action<PartieDeChasse> assert)
    {
        assert(partieDeChasse);
        return partieDeChasse;
    }
}

public static class ChasseurExtensions
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

public static class TerrainExtensions
{
    public static Terrain GalinettesRestantes(this Terrain terrain, int nbGalinettes)
    {
        terrain
            .NbGalinettes
            .Should()
            .Be(nbGalinettes, $"Le terrain devrait contenir {nbGalinettes} mais en contient {terrain.NbGalinettes}");

        return terrain;
    }
}