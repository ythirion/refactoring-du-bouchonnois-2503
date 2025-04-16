using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class PrendreLApéro
{
    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
            {
                ChasseurBuilder.UnChasseurNomméDédé(),
                ChasseurBuilder.UnChasseurNomméBernard(),
                ChasseurBuilder.UnChasseurNomméRobert()
            })
            .Build();
        repository.Add(partieDeChasse);
        var now = DateTime.Now;

        var service = new PartieDeChasseService(repository, () => now);
        service.PrendreLapéro(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();

        savedPartieDeChasse.Should().BeEquivalentTo(new PartieDeChasseBuilder()
            .AyantLId(partieDeChasse.Id)
            .QuiEstApero()
            .AvecDesChasseurs(new List<Chasseur>
            {
                ChasseurBuilder.UnChasseurNomméDédé(),
                ChasseurBuilder.UnChasseurNomméBernard(),
                ChasseurBuilder.UnChasseurNomméRobert()
            })
            .AvecSesEvenements(new List<Event>
            {
                new(now,
                    "Petit apéro")
            })
            .Build()
        );
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
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstApero()
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLApéroQuandOnPrendDéjàLapéro = () => service.PrendreLapéro(partieDeChasse.Id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should()
            .Throw<OnEstDéjàEnTrainDePrendreLapéro>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstTerminee()
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var prendreLapéroQuandTerminée = () => service.PrendreLapéro(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }
}
