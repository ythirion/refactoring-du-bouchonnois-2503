using Bouchonnois.Service;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases;

namespace Bouchonnois.Tests.Acceptances;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly PartieDeChasseService _;
    private readonly TirerSurGalinetteUseCase _tirerSurGalinetteUseCase;
    private readonly TirerUseCase _tirerUseCase;
    private readonly PrendreLapéroUseCase _prendreLapéroUseCase;
    private readonly ReprendreLaPartieDeChasseUseCase _reprendreLaPartieDeChasseUse;
    private readonly TerminerLaPartieUseCase _terminerLaPartieUseCase;
    private readonly ConsulterStatusUseCase _consulterStatusUseCase;
    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests()
    {
        _ = new PartieDeChasseService(_repository, () => _time);
        _tirerSurGalinetteUseCase = new TirerSurGalinetteUseCase(_repository, () => _time);
        _tirerUseCase = new TirerUseCase(_repository, () => _time);
        _prendreLapéroUseCase = new PrendreLapéroUseCase(_repository, () => _time);
        _reprendreLaPartieDeChasseUse = new ReprendreLaPartieDeChasseUseCase(_repository, () => _time);
        _terminerLaPartieUseCase = new TerminerLaPartieUseCase(_repository, () => _time);
        _consulterStatusUseCase = new ConsulterStatusUseCase(_repository);
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

        After(10.MinutesLater(), () => _tirerUseCase.Tirer(id, "Dédé"));
        After(30.MinutesLater(), () => _tirerSurGalinetteUseCase.TirerSurUneGalinette(id, "Robert"));
        After(20.MinutesLater(), () => _prendreLapéroUseCase.PrendreLapéro(id));
        After(1.HoursLater(), () => _reprendreLaPartieDeChasseUse.ReprendreLaPartie(id));
        After(2.MinutesLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.MinutesLater(), () => _tirerSurGalinetteUseCase.TirerSurUneGalinette(id, "Dédé"));
        After(26.MinutesLater(), () => _tirerSurGalinetteUseCase.TirerSurUneGalinette(id, "Robert"));
        After(10.MinutesLater(), () => _prendreLapéroUseCase.PrendreLapéro(id));
        After(170.MinutesLater(), () => _reprendreLaPartieDeChasseUse.ReprendreLaPartie(id));
        After(11.MinutesLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(1.SecondsLater(), () => _tirerUseCase.Tirer(id, "Bernard"));
        After(19.MinutesLater(), () => _tirerSurGalinetteUseCase.TirerSurUneGalinette(id, "Robert"));
        After(30.MinutesLater(), () => _terminerLaPartieUseCase.TerminerLaPartie(id));
        // @formatter:on

        await Verify(_consulterStatusUseCase.ConsulterStatus(id));
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
