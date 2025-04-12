using Bouchonnois.Service;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests;

public class ScenarioTests
{
    private readonly PartieDeChasseRepositoryForTests _repository = new();
    private readonly PartieDeChasseService _service;
    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests() => _service = new PartieDeChasseService(_repository, () => _time);

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

        var id = _service.Demarrer(
            terrainDeChasse,
            chasseurs);

        _time = _time.Add(TimeSpan.FromMinutes(10));
        _service.Tirer(id, "Dédé");

        _time = _time.Add(TimeSpan.FromMinutes(30));
        _service.TirerSurUneGalinette(id, "Robert");

        _time = _time.Add(TimeSpan.FromMinutes(20));
        _service.PrendreLapéro(id);

        _time = _time.Add(TimeSpan.FromHours(1));
        _service.ReprendreLaPartie(id);

        _time = _time.Add(TimeSpan.FromMinutes(2));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromMinutes(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromMinutes(1));
        _service.TirerSurUneGalinette(id, "Dédé");

        _time = _time.Add(TimeSpan.FromMinutes(26));
        _service.TirerSurUneGalinette(id, "Robert");

        _time = _time.Add(TimeSpan.FromMinutes(10));
        _service.PrendreLapéro(id);

        _time = _time.Add(TimeSpan.FromMinutes(170));
        _service.ReprendreLaPartie(id);

        _time = _time.Add(TimeSpan.FromMinutes(11));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));
        _service.Tirer(id, "Bernard");

        _time = _time.Add(TimeSpan.FromSeconds(1));

        try
        {
            _service.Tirer(id, "Bernard");
        }
        catch (TasPlusDeBallesMonVieuxChasseALaMain)
        {
        }

        _time = _time.Add(TimeSpan.FromMinutes(19));
        _service.TirerSurUneGalinette(id, "Robert");

        _time = _time.Add(TimeSpan.FromMinutes(30));
        _service.TerminerLaPartie(id);

        _service.ConsulterStatus(id)
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
}