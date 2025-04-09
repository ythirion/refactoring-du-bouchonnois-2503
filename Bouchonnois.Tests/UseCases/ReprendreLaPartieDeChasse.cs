using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse
{
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                        new(nom: "Dédé", ballesRestantes: 20),
                        new(nom: "Bernard", ballesRestantes: 8),
                        new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Apéro,
                events: new List<Event>()));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        service.ReprendreLaPartie(id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.EnCours);
        savedPartieDeChasse.Terrain.Nom.Should().Be("Pitibon sur Sauldre");
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var reprendrePartieQuandPartieExistePas = () => service.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                        new(nom: "Dédé", ballesRestantes: 20),
                        new(nom: "Bernard", ballesRestantes: 8),
                        new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.EnCours));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var reprendreLaPartieQuandChasseEnCours = () => service.ReprendreLaPartie(id);

        reprendreLaPartieQuandChasseEnCours.Should()
            .Throw<LaChasseEstDéjàEnCours>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                        new(nom: "Dédé", ballesRestantes: 20),
                        new(nom: "Bernard", ballesRestantes: 8),
                        new(nom: "Robert", ballesRestantes: 12),
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Terminée));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.ReprendreLaPartie(id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }
}
