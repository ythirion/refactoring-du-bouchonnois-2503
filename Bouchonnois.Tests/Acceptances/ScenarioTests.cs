using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases.Commands;
using Bouchonnois.UseCases.Queries;

namespace Bouchonnois.Tests.Acceptances;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    private readonly DemarrerUseCase _demarrer;
    private readonly TirerUseCase _tirer;
    private readonly TirerSurGalinetteUseCase _tirerSurGalinette;
    private readonly PrendreLAperoUseCase _prendreLApero;
    private readonly ReprendreLaPartieUseCase _reprendreLaPartie;
    private readonly TerminerLaPartieUseCase _terminerLaPartie;
    private readonly ConsulterStatusUseCase _consulterStatus;

    public ScenarioTests()
    {
        _demarrer = new DemarrerUseCase(_repository, () => _time);
        _tirer = new TirerUseCase(_repository, () => _time);
        _tirerSurGalinette = new TirerSurGalinetteUseCase(_repository, () => _time);
        _prendreLApero = new PrendreLAperoUseCase(_repository, () => _time);
        _reprendreLaPartie = new ReprendreLaPartieUseCase(_repository, () => _time);
        _terminerLaPartie = new TerminerLaPartieUseCase(_repository, () => _time);
        _consulterStatus = new ConsulterStatusUseCase(_repository);
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

        var id = _demarrer.Handle(terrainDeChasse, chasseurs);

        After(10.MinutesLater(), () => _tirer.Handle(id, "Dédé"));
        After(30.MinutesLater(), () => _tirerSurGalinette.Handle(id, "Robert"));
        After(20.MinutesLater(), () => _prendreLApero.HandleWithoutException(id));
        After(1.HoursLater(), () => _reprendreLaPartie.Handle(id));
        After(2.MinutesLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.MinutesLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.MinutesLater(), () => _tirerSurGalinette.Handle(id, "Dédé"));
        After(26.MinutesLater(), () => _tirerSurGalinette.Handle(id, "Robert"));
        After(10.MinutesLater(), () => _prendreLApero.HandleWithoutException(id));
        After(170.MinutesLater(), () => _reprendreLaPartie.Handle(id));
        After(11.MinutesLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Handle(id, "Bernard"));
        After(19.MinutesLater(), () => _tirerSurGalinette.Handle(id, "Robert"));
        After(30.MinutesLater(), () => _terminerLaPartie.Handle(id));
        // @formatter:on

        await Verify(_consulterStatus.Handle(id));
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
