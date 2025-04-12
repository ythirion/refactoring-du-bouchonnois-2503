using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.UseCases;

public class PartieDeChasseBuilder
{
    private PartieStatus _status = PartieStatus.EnCours;
    private List<Chasseur> _chasseurs = new();
    private List<Event> _events = new();

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
            id: id,
            chasseurs: _chasseurs,
            terrain: new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3),
            status: _status,
            events: _events);
    }
}
