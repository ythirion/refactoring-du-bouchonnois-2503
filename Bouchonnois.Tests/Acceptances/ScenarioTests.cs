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
    public async Task DéroulerUnePartie()
    {
        var chasseurs = new List<(string, int)>
        {
            ("Dédé", 20),
            ("Bernard", 8),
            ("Robert", 12)
        };
        var terrainDeChasse = ("Pitibon sur Sauldre", 4);

        var id = _.Demarrer(terrainDeChasse, chasseurs);

        After(10.MinutesLater(), () => _.Tirer(id, "Dédé"));
        After(30.MinutesLater(), () => _.TirerSurUneGalinette(id, "Robert"));
        After(20.MinutesLater(), () => _.PrendreLapéro(id));
        After(1.HoursLater(), () => _.ReprendreLaPartie(id));
        After(2.MinutesLater(), () => _.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _.TirerSurUneGalinette(id, "Dédé"));
        After(26.MinutesLater(), () => _.TirerSurUneGalinette(id, "Robert"));
        After(10.MinutesLater(), () => _.PrendreLapéro(id));
        After(170.MinutesLater(), () => _.ReprendreLaPartie(id));
        After(11.MinutesLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _.Tirer(id, "Bernard"));
        After(19.MinutesLater(), () => _.TirerSurUneGalinette(id, "Robert"));
        After(30.MinutesLater(), () => _.TerminerLaPartie(id));
        // @formatter:on

        await Verify(_.ConsulterStatus(id));
    }

    private void After(TimeSpan time, Action scenarioAction)
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
