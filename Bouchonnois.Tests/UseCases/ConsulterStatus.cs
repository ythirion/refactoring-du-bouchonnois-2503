using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class ConsulterStatus
{
    [Fact]
    public void QuandLaPartieVientDeDémarrer()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var partieDeChasse = new PartieDeChasseBuilder()
            .QuiEstEnCours()
            .AvecDesChasseurs(new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 2),
                }
            )
            .AvecSesEvenements(new List<Event>
            {
                new(
                    new DateTime(2024, 4, 25, 9, 0, 12),
                    "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
            })
            .Build();

        repository.Add(partieDeChasse);

        var status = service.ConsulterStatus(partieDeChasse.Id);

        status.Should()
            .Be(
                "09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)");
    }

    [Fact]
    public void QuandLaPartieEstTerminée()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);

        repository.Add(
            new PartieDeChasse(
                id: id,
                chasseurs: new List<Chasseur>
                {
                    new(nom: "Dédé", ballesRestantes: 20),
                    new(nom: "Bernard", ballesRestantes: 8),
                    new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 2),
                },
                terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
                status: PartieStatus.EnCours,
                events: new List<Event>
                {
                    new(
                        new DateTime(2024, 4, 25, 9, 0, 12),
                        "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"),
                    new(new DateTime(2024, 4, 25, 9, 10, 0), "Dédé tire"),
                    new(new DateTime(2024, 4, 25, 9, 40, 0), "Robert tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 10, 0, 0), "Petit apéro"),
                    new(new DateTime(2024, 4, 25, 11, 0, 0), "Reprise de la chasse"),
                    new(new DateTime(2024, 4, 25, 11, 2, 0), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 11, 3, 0), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 11, 4, 0), "Dédé tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 11, 30, 0), "Robert tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 11, 40, 0), "Petit apéro"),
                    new(new DateTime(2024, 4, 25, 14, 30, 0), "Reprise de la chasse"),
                    new(new DateTime(2024, 4, 25, 14, 41, 0), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 1), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 2), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 3), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 4), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 5), "Bernard tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 6), "Bernard tire"),
                    new(
                        new DateTime(2024, 4, 25, 14, 41, 7),
                        "Bernard tire -> T'as plus de balles mon vieux, chasse à la main"),
                    new(new DateTime(2024, 4, 25, 15, 0, 0), "Robert tire sur une galinette"),
                    new(
                        new DateTime(2024, 4, 25, 15, 30, 0),
                        "La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes"),
                }));

        var status = service.ConsulterStatus(id);

        status.Should()
            .BeEquivalentTo(
                @"15:30 - La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes
15:00 - Robert tire sur une galinette
14:41 - Bernard tire -> T'as plus de balles mon vieux, chasse à la main
14:41 - Bernard tire
14:41 - Bernard tire
14:41 - Bernard tire
14:41 - Bernard tire
14:41 - Bernard tire
14:41 - Bernard tire
14:41 - Bernard tire
14:30 - Reprise de la chasse
11:40 - Petit apéro
11:30 - Robert tire sur une galinette
11:04 - Dédé tire sur une galinette
11:03 - Bernard tire
11:02 - Bernard tire
11:00 - Reprise de la chasse
10:00 - Petit apéro
09:40 - Robert tire sur une galinette
09:10 - Dédé tire
09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)");
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new PartieDeChasseService(repository, () => DateTime.Now);
        var reprendrePartieQuandPartieExistePas = () => service.ConsulterStatus(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }
}

public class PartieDeChasseBuilder
{
    private PartieStatus _status = PartieStatus.EnCours;
    private List<Chasseur> _chasseurs = new();
    private List<Event> _events = new();

    public PartieDeChasseBuilder QuiEstEnCours()
    {
        _status = PartieStatus.EnCours;
        return this;
    }

    public PartieDeChasseBuilder AvecDesChasseurs(List<Chasseur> chasseurs)
    {
        _chasseurs = chasseurs;
        return this;
    }

    public PartieDeChasseBuilder AvecSesEvenements(List<Event> events)
    {
        _events = events;
        return this;
    }

    public PartieDeChasse Build()
    {
        var id = Guid.NewGuid();
        return new PartieDeChasse(
            id: id,
            chasseurs: _chasseurs,
            terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
            status: _status,
            events: _events);
    }
}
