using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases;

namespace Bouchonnois.Tests.Acceptances;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly DemarrerUnePartieDeChasseUseCase _demarrerUnePartie;
    private readonly TirerSurGalinetteUseCase _tirerSurGalinette;
    private readonly TirerUseCase _tirer;
    private readonly PrendreLapéroUseCase _prendreLapéro;
    private readonly ReprendreLaPartieDeChasseUseCase _reprendreLaPartie;
    private readonly TerminerLaPartieDeChasseUseCase _terminerLaPartie;
    private readonly ConsulterStatusUseCase _consulterStatus;

    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests()
    {
        _demarrerUnePartie = new DemarrerUnePartieDeChasseUseCase(_repository, () => _time);
        _tirer = new TirerUseCase(_repository, () => _time);
        _tirerSurGalinette = new TirerSurGalinetteUseCase(_repository, () => _time);
        _prendreLapéro = new PrendreLapéroUseCase(_repository, () => _time);
        _reprendreLaPartie = new ReprendreLaPartieDeChasseUseCase(_repository, () => _time);
        _terminerLaPartie = new TerminerLaPartieDeChasseUseCase(_repository, () => _time);
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

        var id = _demarrerUnePartie.Demarrer(terrainDeChasse, chasseurs);

        After(10.MinutesLater(), () => _tirer.Tirer(id, "Dédé"));
        After(30.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
        After(20.MinutesLater(), () => _prendreLapéro.PrendreLapéro(id));
        After(1.HoursLater(), () => _reprendreLaPartie.ReprendreLaPartie(id));
        After(2.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Dédé"));
        After(26.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
        After(10.MinutesLater(), () => _prendreLapéro.PrendreLapéro(id));
        After(170.MinutesLater(), () => _reprendreLaPartie.ReprendreLaPartie(id));
        After(11.MinutesLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirer.Tirer(id, "Bernard"));
        After(19.MinutesLater(), () => _tirerSurGalinette.TirerSurUneGalinette(id, "Robert"));
        After(30.MinutesLater(), () => _terminerLaPartie.TerminerLaPartie(id));

        await Verify(_consulterStatus.ConsulterStatus(id));
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