using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class DemarrerUnePartieDeChasse
{
    [Fact]
    public void AvecPlusieursChasseurs()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var now = DateTime.Now;
        var service = new PartieDeChasseService(repository, () => now);
        var chasseurs = new List<(string, int)>
        {
            ("Dédé", 20),
            ("Bernard", 8),
            ("Robert", 12)
        };
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);
        var id = service.Demarrer(
            terrainDeChasse,
            chasseurs
        );

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Should().BeEquivalentTo(new PartieDeChasseBuilder()
            .AyantLId(id)
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    ChasseurBuilder.UnChasseurNomméDédé(),
                    ChasseurBuilder.UnChasseurNomméBernard(),
                    ChasseurBuilder.UnChasseurNomméRobert(),
                }
            )
            .AvecSesEvenements(new List<Event>
            {
                new(now,
                    "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
            })
            .Build()
        );
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
            ("Dédé", 20),
            ("Bernard", 0),
        };
        var terrainDeChasse = ("Pitibon sur Sauldre", 3);

        Action demarrerPartieAvecChasseurSansBalle = () => service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should()
            .Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }
}
