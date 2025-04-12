using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Assertions
{
    public static class PartieDeChasseAssertions
    {
        public static PartieDeChasse HaveEmitted(this PartieDeChasse partieDeChasse, string expectedMessage)
            => Assert(partieDeChasse, p =>
                p.Events
                    .Should()
                    .HaveCount(1)
                    .And
                    .ContainSingle(e => e.Message == expectedMessage,
                        $"Les events devraient contenir {expectedMessage}.")
            );


        private static Chasseur Chasseur(this PartieDeChasse partieDeChasse, string nom)
            => partieDeChasse
                .Chasseurs
                .Should()
                .ContainSingle(c => c.Nom == nom, "Chasseur non présent dans la partie de chasse")
                .Subject;

        public static PartieDeChasse ChasseurATiréSurUneGalinette(
            this PartieDeChasse partieDeChasse,
            string nom,
            int ballesRestantes,
            int galinettes) =>
            Assert(partieDeChasse, p =>
                p.Chasseur(nom)
                    .BallesRestantes(ballesRestantes)
                    .ATué(galinettes)
            );


        public static PartieDeChasse ChasseurATiré(
            this PartieDeChasse partieDeChasse,
            string nom,
            int ballesRestantes) =>
            Assert(partieDeChasse, p =>
                p.Chasseur(nom)
                    .BallesRestantes(ballesRestantes)
            );

        public static PartieDeChasse GalinettesRestantesSurLeTerrain(this PartieDeChasse partieDeChasse,
            int nbGalinettes) =>
            Assert(partieDeChasse, p =>
                p.Terrain
                    .GalinettesRestantes(nbGalinettes)
            );

        public static PartieDeChasse BeInApéro(this PartieDeChasse partieDeChasse) =>
            Assert(partieDeChasse,
                p => p.Status.Should().Be(PartieStatus.Apéro, "Les chasseurs devraient être à l'apéro")
            );

        public static PartieDeChasse BeEnCours(this PartieDeChasse partieDeChasse) =>
            Assert(partieDeChasse,
                p => p.Status.Should().Be(PartieStatus.EnCours, "Les chasseurs devraient être en cours de chasse")
            );

        public static PartieDeChasse BeTerminé(this PartieDeChasse partieDeChasse) =>
            Assert(partieDeChasse,
                p => p.Status.Should().Be(PartieStatus.Terminée, "La partie de Chasse devrait être terminée")
            );

        private static PartieDeChasse Assert(PartieDeChasse partieDeChasse, Action<PartieDeChasse> assert)
        {
            assert(partieDeChasse);
            return partieDeChasse;
        }
    }
}
