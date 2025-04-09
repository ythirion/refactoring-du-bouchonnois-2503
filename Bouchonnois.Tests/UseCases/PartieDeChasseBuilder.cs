using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.UseCases;

public class PartieDeChasseBuilder
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly Terrain _terrain = new("Pitibon sur Sauldre", 3);

    private List<Chasseur> _chasseurs =
    [
        new("Dédé", 20),
        new("Bernard", 8),
        new("Robert", 12)
    ];

    private List<Event> _events = [];
    private PartieStatus _status = PartieStatus.EnCours;

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

    public PartieDeChasseBuilder AvecSesEvenements(params List<Event> events)
    {
        _events = events;
        return this;
    }

    public PartieDeChasse Build()
        => new(
            _id,
            chasseurs: _chasseurs,
            terrain: _terrain,
            status: _status,
            events: _events);
}