using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;
using static Bouchonnois.Tests.Builders.PartieDeChasseBuilder;

namespace Bouchonnois.Tests.UseCases;

public class ConsulterStatus : UseCaseTest
{
    [Fact]
    public void QuandLaPartieVientDeDémarrer()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .AvecEvenements(
                    new Event(
                        new DateTime(2024, 4, 25, 9, 0, 12),
                        "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")));


        var status = Service.ConsulterStatus(id);

        status.Should()
            .Be(
                "09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)");
    }

    [Fact]
    public void QuandLaPartieEstTerminée()
    {
        var id = UnePartieDeChasseExistante(
            UnePartieDeChasse()
                .AvecEvenements(
                    // @formatter:off
                    new (new (2024, 04, 25, 09, 00, 12), "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"),
                    new (new (2024, 04, 25, 09, 10, 00), "Dédé tire"),
                    new (new (2024, 04, 25, 09, 40, 00), "Robert tire sur une galinette"),
                    new (new (2024, 04, 25, 10, 00, 00), "Petit apéro"),
                    new (new (2024, 04, 25, 11, 00, 00), "Reprise de la chasse"),
                    new (new (2024, 04, 25, 11, 02, 00), "Bernard tire"),
                    new (new (2024, 04, 25, 11, 03, 00), "Bernard tire"),
                    new (new (2024, 04, 25, 11, 04, 00), "Dédé tire sur une galinette"),
                    new (new (2024, 04, 25, 11, 30, 00), "Robert tire sur une galinette"),
                    new (new (2024, 04, 25, 11, 40, 00), "Petit apéro"),
                    new (new (2024, 04, 25, 14, 30, 00), "Reprise de la chasse"),
                    new (new (2024, 04, 25, 14, 41, 00), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 01), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 02), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 03), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 04), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 05), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 06), "Bernard tire"),
                    new (new (2024, 04, 25, 14, 41, 07), "Bernard tire -> T'as plus de balles mon vieux, chasse à la main"),
                    new (new (2024, 04, 25, 15, 00, 00), "Robert tire sur une galinette"),
                    new (new (2024, 04, 25, 15, 30, 00), "La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes")));
        // @formatter:on

        var status = Service.ConsulterStatus(id);

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