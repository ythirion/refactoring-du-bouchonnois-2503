using Bouchonnois.Service;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Acceptances;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly PartieDeChasseService _;
    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests() => _ = new PartieDeChasseService(_repository, () => _time);

    [Fact]
    public void DéroulerUnePartie()
    {
        var chasseurs = new List<(string, int)>
        {
            ("Dédé", 20),
            ("Bernard", 8),
            ("Robert", 12)
        };
        var terrainDeChasse = ("Pitibon sur Sauldre", 4);

        var id = _.Demarrer(terrainDeChasse, chasseurs);

        // @formatter:off
        Act(10.MinutesLater(),  () => _.Tirer(id, "Dédé"));
        Act(30.MinutesLater(),  () => _.TirerSurUneGalinette(id, "Robert"));
        Act(20.MinutesLater(),  () => _.PrendreLapéro(id));
        Act(1.HoursLater(),     () => _.ReprendreLaPartie(id));
        Act(2.MinutesLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.MinutesLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.MinutesLater(),   () => _.TirerSurUneGalinette(id, "Dédé"));
        Act(26.MinutesLater(),  () => _.TirerSurUneGalinette(id, "Robert"));
        Act(10.MinutesLater(),  () => _.PrendreLapéro(id));
        Act(170.MinutesLater(), () => _.ReprendreLaPartie(id));
        Act(11.MinutesLater(),  () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(1.SecondsLater(),   () => _.Tirer(id, "Bernard"));
        Act(19.MinutesLater(),  () => _.TirerSurUneGalinette(id, "Robert"));
        Act(30.MinutesLater(),  () => _.TerminerLaPartie(id));
        // @formatter:on

        _.ConsulterStatus(id)
            .Should()
            .BeEquivalentTo(
                """
                15:30 - La partie de chasse est terminée, vainqueur : Robert - 3 galinettes
                15:00 - Robert tire sur une galinette
                14:41 - Bernard tire -> T'as plus de balles mon vieux, chasse à la main
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
                09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)
                """);
    }
    
    private void Act(TimeSpan time, Action scenarioAction)
    {
        _time = _time.Add(time);

        try
        {
            scenarioAction();
        }
        catch
        {
            // ignored
        }
    }
}