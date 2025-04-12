using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Assertions;
using Bouchonnois.Tests.DataBuilders;
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
                    new(nom: Data.Dede, ballesRestantes: 20),
                    new(nom: Data.Bernard, ballesRestantes: 8),
                    new(nom: Data.Robert, ballesRestantes: 12, nbGalinettes: 2),
                }
            )
            .AvecSesEvenements(new List<Event>
            {
                new(
                    new DateTime(2024, 4, 25, 9, 0, 12),
                    $"La partie de chasse commence à {Data.Terrain} avec {Data.Dede} (20 balles), {Data.Bernard} (8 balles), {Data.Robert} (12 balles)")
            })
            .Build();

        repository.Add(partieDeChasse);

        var status = service.ConsulterStatus(partieDeChasse.Id);

        status.Should()
            .Be(
                $"09:00 - La partie de chasse commence à {Data.Terrain} avec {Data.Dede} (20 balles), {Data.Bernard} (8 balles), {Data.Robert} (12 balles)");
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
                    new(nom: Data.Dede, ballesRestantes: 20),
                    new(nom: Data.Bernard, ballesRestantes: 8),
                    new(nom: Data.Robert, ballesRestantes: 12, nbGalinettes: 2),
                },
                terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
                status: PartieStatus.EnCours,
                events: new List<Event>
                {
                    new(
                        new DateTime(2024, 4, 25, 9, 0, 12),
                        $"La partie de chasse commence à {Data.Terrain} avec {Data.Dede} (20 balles), {Data.Bernard} (8 balles), {Data.Robert} (12 balles)"),
                    new(new DateTime(2024, 4, 25, 9, 10, 0), $"{Data.Dede} tire"),
                    new(new DateTime(2024, 4, 25, 9, 40, 0), $"{Data.Robert} tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 10, 0, 0), "Petit apéro"),
                    new(new DateTime(2024, 4, 25, 11, 0, 0), "Reprise de la chasse"),
                    new(new DateTime(2024, 4, 25, 11, 2, 0), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 11, 3, 0), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 11, 4, 0), $"{Data.Dede} tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 11, 30, 0), $"{Data.Robert} tire sur une galinette"),
                    new(new DateTime(2024, 4, 25, 11, 40, 0), "Petit apéro"),
                    new(new DateTime(2024, 4, 25, 14, 30, 0), "Reprise de la chasse"),
                    new(new DateTime(2024, 4, 25, 14, 41, 0), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 1), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 2), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 3), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 4), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 5), $"{Data.Bernard} tire"),
                    new(new DateTime(2024, 4, 25, 14, 41, 6), $"{Data.Bernard} tire"),
                    new(
                        new DateTime(2024, 4, 25, 14, 41, 7),
                        $"{Data.Bernard} tire -> T'as plus de balles mon vieux, chasse à la main"),
                    new(new DateTime(2024, 4, 25, 15, 0, 0), $"{Data.Robert} tire sur une galinette"),
                    new(
                        new DateTime(2024, 4, 25, 15, 30, 0),
                        $"La partie de chasse est terminée, vainqueur :  {Data.Robert} - 3 galinettes"),
                }));

        var status = service.ConsulterStatus(id);

        status.Should()
            .BeEquivalentTo(
                @$"15:30 - La partie de chasse est terminée, vainqueur :  {Data.Robert} - 3 galinettes
15:00 - {Data.Robert} tire sur une galinette
14:41 - {Data.Bernard} tire -> T'as plus de balles mon vieux, chasse à la main
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:41 - {Data.Bernard} tire
14:30 - Reprise de la chasse
11:40 - Petit apéro
11:30 - {Data.Robert} tire sur une galinette
11:04 - {Data.Dede} tire sur une galinette
11:03 - {Data.Bernard} tire
11:02 - {Data.Bernard} tire
11:00 - Reprise de la chasse
10:00 - Petit apéro
09:40 - {Data.Robert} tire sur une galinette
09:10 - {Data.Dede} tire
09:00 - La partie de chasse commence à {Data.Terrain} avec {Data.Dede} (20 balles), {Data.Bernard} (8 balles), {Data.Robert} (12 balles)");
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
