using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Verifications;

public static class PartieDeChasseVerificationExtensions
{
    public static PartieDeChasse DevraitAvoirEmis(
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

    public static PartieDeChasse ChasseurDevraitAvoirTiréSurUneGalinette(
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


    public static PartieDeChasse ChasseurDevraitAvoirTiré(
        this PartieDeChasse partieDeChasse,
        string nom,
        int ballesRestantes)
        => Assert(
            partieDeChasse,
            p =>
                p.Chasseur(nom)
                    .BallesRestantes(ballesRestantes));

    public static PartieDeChasse TerrainDevraitAvoirGalinettesRestantes(
        this PartieDeChasse partieDeChasse,
        int nbGalinettes)
        => Assert(
            partieDeChasse,
            p =>
                p.Terrain.GalinettesRestantes(nbGalinettes));

    public static PartieDeChasse DevraitEtreALApéro(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.Apéro, "Les chasseurs devraient être à l'apéro"));

    public static PartieDeChasse DevraitEtreEnCours(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.EnCours, "Les chasseurs devraient être en cours de chasse"));

    public static PartieDeChasse DevraitEtreTerminé(this PartieDeChasse partieDeChasse)
        => Assert(
            partieDeChasse,
            p => p.Status.Should().Be(PartieStatus.Terminée, "La partie de Chasse devrait être terminée"));

    private static PartieDeChasse Assert(PartieDeChasse partieDeChasse, Action<PartieDeChasse> assert)
    {
        assert(partieDeChasse);
        return partieDeChasse;
    }
}