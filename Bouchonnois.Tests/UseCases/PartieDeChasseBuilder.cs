using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.UseCases;

public class PartieDeChasseBuilder
{
    private List<Chasseur> _chasseurs = new();
    private List<Event> _events = new();
    private PartieStatus _status = PartieStatus.EnCours;

    private PartieDeChasseBuilder()
    {
    }

    public static PartieDeChasseBuilder UnePartieDeChasseEnCours => new() { _status = PartieStatus.EnCours };

    public static PartieDeChasseBuilder UnePartieDeChasseALapero => new() { _status = PartieStatus.Apéro };

    public static PartieDeChasseBuilder UnePartieDeChasseTerminée => new() { _status = PartieStatus.Terminée };

    [Obsolete("Use UnePartieDeChasseEnCours instead", true)]
    public PartieDeChasseBuilder QuiEstEnCours()
    {
        _status = PartieStatus.EnCours;
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
            terrain: new Terrain("Pitibon sur Sauldre", Data.GalinettesSurUnTerrainRiche),
            status: _status,
            events: _events);
    }
}
