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
                    .AtHour(9).AndSeconds(12).Build()
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
                    .AtHour(9).AndSeconds(12).Build(),
                new EventBuilder("Dédé tire").AtHour(9).AndMinutes(10).Build(),
                new EventBuilder("Robert tire sur une galinette").AtHour(9).AndMinutes(40).Build(),
                new EventBuilder("Petit apéro").AtHour(10).Build(),
                new EventBuilder("Reprise de la chasse").AtHour(11).Build(),
                new EventBuilder("Bernard tire").AtHour(11).AndMinutes(2).Build(),
                new EventBuilder("Bernard tire").AtHour(11).AndMinutes(3).Build(),
                new EventBuilder("Dédé tire sur une galinette").AtHour(11).AndMinutes(4).Build(),
                new EventBuilder("Robert tire sur une galinette").AtHour(11).AndMinutes(30).Build(),
                new EventBuilder("Petit apéro").AtHour(11).AndMinutes(40).Build(),
                new EventBuilder("Reprise de la chasse").AtHour(14).AndMinutes(30).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(1).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(2).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(3).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(4).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(5).Build(),
                new EventBuilder("Bernard tire").AtHour(14).AndMinutes(41).AndSeconds(6).Build(),
                new EventBuilder("Bernard tire -> T'as plus de balles mon vieux, chasse à la main")
                    .AtHour(14).AndMinutes(41).AndSeconds(7).Build(),
                new EventBuilder("Robert tire sur une galinette").AtHour(15).Build(),
                new EventBuilder("La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes")
                    .AtHour(15).AndMinutes(30).Build()
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
