using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Abstractions;
using Bouchonnois.Tests.UseCases.DataBuilders;

namespace Bouchonnois.Tests.UseCases;

public class ConsulterStatus : PartieDeChasseBaseTests
{
    [Fact]
    public void QuandLaPartieVientDeDémarrer()
    {
        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([
                Dédé,
                Bernard,
                Robert.AvecDesGalinettesDansSaBesace(2)
            ])
            .AvecSesEvenements(new List<Event>
            {
                new EventBuilder(
                        "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
                    .Hour(9).Seconds(12)
            })
            .Build();

        Repository.Add(partieDeChasse);

        var status = service.ConsulterStatus(partieDeChasse.Id);

        status.Should()
            .Be("09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)");
    }

    [Fact]
    public void QuandLaPartieEstTerminée()
    {
        var service = new PartieDeChasseService(Repository, () => DateTime.Now);

        var partieDeChasse = UnePartieDeChasseEnCours
            .AvecDesChasseurs([
                Dédé,
                Bernard,
                Robert.AvecDesGalinettesDansSaBesace(2)
            ])
            .AvecSesEvenements([
                new EventBuilder(
                        "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
                    .Hour(9).Seconds(12),
                new EventBuilder("Dédé tire").Hour(9).Minutes(10),
                new EventBuilder("Robert tire sur une galinette").Hour(9).Minutes(40),
                new EventBuilder("Petit apéro").Hour(10),
                new EventBuilder("Reprise de la chasse").Hour(11),
                new EventBuilder("Bernard tire").Hour(11).Minutes(2),
                new EventBuilder("Bernard tire").Hour(11).Minutes(3),
                new EventBuilder("Dédé tire sur une galinette").Hour(11).Minutes(4),
                new EventBuilder("Robert tire sur une galinette").Hour(11).Minutes(30),
                new EventBuilder("Petit apéro").Hour(11).Minutes(40),
                new EventBuilder("Reprise de la chasse").Hour(14).Minutes(30),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(1),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(2),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(3),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(4),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(5),
                new EventBuilder("Bernard tire").Hour(14).Minutes(41).Seconds(6),
                new EventBuilder("Bernard tire -> T'as plus de balles mon vieux, chasse à la main")
                    .Hour(14).Minutes(41).Seconds(7),
                new EventBuilder("Robert tire sur une galinette").Hour(15),
                new EventBuilder("La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes")
                    .Hour(15).Minutes(30)
            ])
            .Build();

        Repository.Add(partieDeChasse);

        var status = service.ConsulterStatus(partieDeChasse.Id);

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
        var service = new PartieDeChasseService(Repository, () => DateTime.Now);
        var reprendrePartieQuandPartieExistePas = () => service.ConsulterStatus(Guid.NewGuid());

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}
