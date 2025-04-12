using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.DataBuilders;

public class PartieDeChasseBuilder
{
    private List<Chasseur> _chasseurs = new List<Chasseur>
    {
        new(nom: "Dédé", ballesRestantes: 20),
        new(nom: "Bernard", ballesRestantes: 8),
        new(nom: "Robert", ballesRestantes: 12, nbGalinettes: 2),
    };

    private Terrain _terrain = new Terrain(nom: "Pitibon sur Sauldre", nbGalinettes: 3);
    private PartieStatus _status = PartieStatus.EnCours;
    private List<Event> _events = new();

    public PartieDeChasseBuilder QuiEstEnCours()
    {
        _status = PartieStatus.EnCours;
        return this;
    }

    public PartieDeChasseBuilder QuiEstApero()
    {
        _status = PartieStatus.Apéro;
        return this;
    }

    public PartieDeChasseBuilder AvecDesChasseurs(List<Chasseur> chasseurs)
    {
        _chasseurs = chasseurs;
        return this;
    }

    public PartieDeChasseBuilder SurUnTerrain(Terrain terrain)
    {
        _terrain = terrain;
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
            terrain: _terrain,
            status: _status,
            events: _events);
    }
}
