using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
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
        var id = Service.Demarrer(
            terrainDeChasse,
            chasseurs);

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

    [Fact]
    public void EchoueSansChasseurs()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        Action demarrerPartieSansChasseurs = () => service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should()
            .Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnTerrainSansGalinettes()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = ("Pitibon sur Sauldre", 0);

        Action demarrerPartieSansChasseurs = () => service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should()
            .Throw<ImpossibleDeDémarrerUnePartieSansGalinettes>();
    }

    [Fact]
    public void EchoueSiChasseurSansBalle()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurs = new List<(string, int)>
        {
            (Dédé, 20),
            ("Bernard", 0)
        };
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        Action demarrerPartieAvecChasseurSansBalle = () => service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should()
            .Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }
}