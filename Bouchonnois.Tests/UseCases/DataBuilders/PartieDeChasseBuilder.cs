using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.UseCases.DataBuilders;

public class PartieDeChasseBuilder
{
    private readonly PartieStatus _status;
    private List<Chasseur> _chasseurs = [];
    private List<Event> _events = [];
    private int _nbGalinettes = Data.GalinettesSurUnTerrainRiche;

    private PartieDeChasseBuilder(PartieStatus status)
    {
        _status = status;
    }

    public static PartieDeChasseBuilder UnePartieDeChasseEnCours => new(PartieStatus.EnCours);

    public static PartieDeChasseBuilder UnePartieDeChasseALapero => new(PartieStatus.Apéro);

    public static PartieDeChasseBuilder UnePartieDeChasseTerminée => new(PartieStatus.Terminée);

    public PartieDeChasseBuilder AvecUnTerrainSansGalinette()
    {
        _nbGalinettes = 0;
        return this;
    }

    public PartieDeChasseBuilder AvecDesChasseurs(List<Chasseur> chasseurs)
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
            chasseurs: _chasseurs,
            terrain: new Terrain("Pitibon sur Sauldre", _nbGalinettes),
            status: _status,
            events: _events);
    }
}
