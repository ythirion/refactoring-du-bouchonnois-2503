using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Verifications;

public static class Verification
{
    public static void VerifierEvenementEmis(this PartieDeChasse partieDeChasse, DateTime now, string message)
        => partieDeChasse
            .Events
            .Should()
            .BeEquivalentTo([new Event(now, message)]);

    public static void VerifierChasseurATiré(this PartieDeChasse partieDeChasse, string nom, int ballesRestantes)
        => partieDeChasse.Chasseurs.First(c => c.Nom == nom).BallesRestantes.Should().Be(ballesRestantes);
}