using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.Verifications;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro : UseCaseTest
{
    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .EnCours());

        Service.PrendreLapéro(id);

        Repository.SavedPartieDeChasse().EstALApéro();
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var apéroQuandPartieExistePas = () => service.PrendreLapéro(id);

        apéroQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
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
                    new(nom: "Robert", ballesRestantes: 12)
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Apéro));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLApéroQuandOnPrendDéjàLapéro = () => service.PrendreLapéro(id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should()
            .Throw<OnEstDéjàEnTrainDePrendreLapéro>();
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
                    new(nom: "Robert", ballesRestantes: 12)
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.Terminée));

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.PrendreLapéro(id);

        prendreLapéroQuandTerminée.Should()
            .Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }
}