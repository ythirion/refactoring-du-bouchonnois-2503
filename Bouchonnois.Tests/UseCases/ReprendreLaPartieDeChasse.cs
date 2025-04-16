using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.DataBuilders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class ReprendreLaPartieDeChasse
{
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var now = DateTime.Now;
        var chasseurs = new List<Chasseur>
        {
            ChasseurBuilder.UnChasseurNomméDédé(),
            ChasseurBuilder.UnChasseurNomméBernard(),
            ChasseurBuilder.UnChasseurNomméRobert()
        };
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstApero()
            .AvecDesChasseurs(chasseurs)
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => now);
        service.ReprendreLaPartie(partieDeChasse.Id);

        var savedPartieDeChasse = repository.SavedPartieDeChasse();


        savedPartieDeChasse.Should().BeEquivalentTo(new PartieDeChasseBuilder()
            .AyantLId(partieDeChasse.Id)
            .QuiEstEnCours()
            .AvecDesChasseurs(chasseurs)
            .AvecSesEvenements(new List<Event>
            {
                new(now,
                    "Reprise de la chasse")
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
        var reprendrePartieQuandPartieExistePas = () => service.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
    {
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .Build();
        repository.Add(partieDeChasse);

        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var reprendreLaPartieQuandChasseEnCours = () => service.ReprendreLaPartie(partieDeChasse.Id);

        reprendreLaPartieQuandChasseEnCours.Should()
            .Throw<LaChasseEstDéjàEnCours>();

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
        var prendreLapéroQuandTerminée = () => service.ReprendreLaPartie(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }
}
