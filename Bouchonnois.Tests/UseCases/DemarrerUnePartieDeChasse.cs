using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.UseCases.Common;

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

using static Bouchonnois.Tests.Builders.ChasseurBuilder;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;
using static Bouchonnois.Tests.Builders.TerrainBuilder;

namespace Bouchonnois.Tests.UseCases;

public class DemarrerUnePartieDeChasse : UseCaseTest
{
    [Fact]
    public void AvecPlusieursChasseurs()
    {
        var chasseurs = new List<(string, int)>
        {
            (Dédé, 20),
            (Bernard, 8),
            (Robert, 12)
        };

        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var id = Service.Demarrer(terrainDeChasse, chasseurs);

        Repository.SavedPartieDeChasse()
            .Should()
            .BeEquivalentTo(
                UnePartieDeChasse()
                    .IdentifiéePar(id)
                    .EnCours()
                    .Sur(UnTerrain().Nommé("Pitibon sur Sauldre").AyantGalinettes(3))
                    .Avec(
                        Dédé().AyantDesBalles(20),
                        Bernard().AyantDesBalles(8),
                        Robert().AyantDesBalles(12))
                    .AvecEvenements(
                        new Event(
                            Now,
                            "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"))
                    .Build());
    }

    [Property]
    public Property DémarrerAvecSucces() => Prop.ForAll(
        Gen.Choose(1, 1000).ToArbitrary(),
        Gen.Choose(1, 1000).ToArbitrary(),
        (chasseurs, balles) => false);

    [Fact]
    public void EchoueSansChasseurs()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var demarrerPartieSansChasseurs = () => Service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should().Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnTerrainSansGalinettes()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 0);

        var demarrerPartieSansChasseurs = () => Service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should().Throw<ImpossibleDeDémarrerUnePartieSansGalinettes>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiChasseurSansBalle()
    {
        var chasseurs = new List<(string, int)>
        {
            (Dédé, 20),
            (Bernard, 0)
        };

        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        var demarrerPartieAvecChasseurSansBalle = () => Service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should().Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
