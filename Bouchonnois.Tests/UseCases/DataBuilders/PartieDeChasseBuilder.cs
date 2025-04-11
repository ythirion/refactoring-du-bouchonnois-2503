using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.UseCases.DataBuilders;

public class PartieDeChasseBuilder
{
    private List<ChasseurBuilder> _chasseurs = new();
    private List<Event> _events = new();
    private PartieStatus _status = PartieStatus.EnCours;
    private int _nbGalinettes = Data.GalinettesSurUnTerrainRiche;

    private PartieDeChasseBuilder()
    {
    }

    public static PartieDeChasseBuilder UnePartieDeChasseEnCours => new() { _status = PartieStatus.EnCours };

    public static PartieDeChasseBuilder UnePartieDeChasseALapero => new() { _status = PartieStatus.Apéro };

    public static PartieDeChasseBuilder UnePartieDeChasseTerminée => new() { _status = PartieStatus.Terminée };

    public PartieDeChasseBuilder AvecDesChasseurs(List<ChasseurBuilder> chasseurs)
    {
        _chasseurs = chasseurs;
        return this;
    }

    public PartieDeChasseBuilder AvecSesEvenements(List<Event> events)
    {
        _events = events;
        return this;
    }

    public PartieDeChasse Build()
    {
        var id = Guid.NewGuid();
        return new PartieDeChasse(
            id,
            chasseurs: _chasseurs.Select(chasseur => chasseur.Build()).ToList(),
            terrain: new Terrain("Pitibon sur Sauldre", _nbGalinettes),
            status: _status,
            events: _events);
    }
    public PartieDeChasseBuilder AvecUnTerrainSansGalinette()
    {
        _nbGalinettes = 0;
        return this;
    }
}
