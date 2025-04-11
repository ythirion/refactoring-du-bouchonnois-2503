using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Verifications;

public static class Verification
{
    public static PartieDeChasse EvenementEmis(this PartieDeChasse partieDeChasse, DateTime now, string message)
        => Assert(
            partieDeChasse,
            p => p
                .Events
                .Should()
                .BeEquivalentTo([new Event(now, message)]));

    public static PartieDeChasse ChasseurATiré(this PartieDeChasse partieDeChasse, string nom, int ballesRestantes)
        => Assert(
            partieDeChasse,
            p
                => p.Chasseurs.First(c => c.Nom == nom).BallesRestantes.Should().Be(ballesRestantes));

    private static PartieDeChasse Assert(PartieDeChasse partieDeChasse, Action<PartieDeChasse> assert)
    {
        assert(partieDeChasse);
        return partieDeChasse;
    }
}