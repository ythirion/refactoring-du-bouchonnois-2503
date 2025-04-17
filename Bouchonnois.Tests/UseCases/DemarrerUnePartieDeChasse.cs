using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class DemarrerUnePartieDeChasse : PartieDeChasseTestBase
{
    [Fact]
    public void AvecPlusieursChasseurs()
    {
        var chasseurs = new List<(string, int)>
        {
            (Data.Dede, 20),
            (Data.Bernard, 8),
            (Data.Robert, 12)
        };
        var terrainDeChasse = (Data.Terrain, 3);
        var id = _service.Demarrer(terrainDeChasse, chasseurs);

        var savedPartieDeChasse = _repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(id);
        savedPartieDeChasse.BeEnCours();//Status.Should().Be(PartieStatus.EnCours);wwww
        savedPartieDeChasse.Terrain.Nom.Should().Be(Data.Terrain);
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be(Data.Dede);
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be(Data.Bernard);
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be(Data.Robert);
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);

        savedPartieDeChasse.Chasseurs.All(chasseur => chasseur.NbGalinettes == 0).Should().BeTrue();
    }

    [Fact]
    public void EchoueSansChasseurs()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = (Data.Terrain, 3);

        Action demarrerPartieSansChasseurs = () => _service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should()
            .Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnTerrainSansGalinettes()
    {
        var chasseurs = new List<(string, int)>();
        var terrainDeChasse = (Data.Terrain, 0);

        Action demarrerPartieSansChasseurs = () => _service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieSansChasseurs.Should()
            .Throw<ImpossibleDeDémarrerUnePartieSansGalinettes>();
    }

    [Fact]
    public void EchoueSiChasseurSansBalle()
    {
        var chasseurs = new List<(string, int)>
        {
            (Data.Dede, 20),
            (Data.Bernard, 0),
        };
        var terrainDeChasse = (Data.Terrain, 3);

        Action demarrerPartieAvecChasseurSansBalle = () => _service.Demarrer(terrainDeChasse, chasseurs);

        demarrerPartieAvecChasseurSansBalle.Should()
            .Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();
        _repository.SavedPartieDeChasse().Should().BeNull();
    }
}

public abstract class PartieDeChasseTestBase
{
    protected readonly PartieDeChasseRepositoryForTests _repository;
    protected readonly PartieDeChasseService _service;

    protected PartieDeChasseTestBase()
    {
        _repository = new PartieDeChasseRepositoryForTests();
        _service = new PartieDeChasseService(_repository, () => DateTime.Now);
    }
}
