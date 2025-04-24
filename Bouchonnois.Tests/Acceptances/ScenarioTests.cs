using Bouchonnois.Service;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Acceptances;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly PartieDeChasseService _;
    private readonly TirerSurGalinetteUseCase _tirerSurGalinette;
    private readonly TirerUseCase _tirer;
    private readonly PrendreLapéroUseCase _prendreLapéro;

    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests()
    {
        _ = new PartieDeChasseService(_repository, () => _time);
        _tirer = new TirerUseCase(_repository, () => _time);
        _tirerSurGalinette = new TirerSurGalinetteUseCase(_repository, () => _time);
        _prendreLapéro = new PrendreLapéroUseCase(_repository, () => _time);
    }


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

        After(10.MinutesLater(), () => _tirer.Tirer(id, "Dédé"));
        After(30.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
        After(20.MinutesLater(), () => _prendreLapéro.PrendreLapéro(id));
        After(1.HoursLater(), () => _.ReprendreLaPartie(id));
        After(2.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Dédé"));
        After(26.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
        After(10.MinutesLater(), () => _prendreLapéro.PrendreLapéro(id));
        After(170.MinutesLater(), () => _.ReprendreLaPartie(id));
        After(11.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(19.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
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