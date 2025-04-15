using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.DataBuilders;

public class PartieDeChasseBuilder
{
    private Guid _id = Guid.NewGuid();

    private List<Chasseur> _chasseurs = new List<Chasseur>
    {
        new ChasseurBuilder().Nommé("Dédé").AyantDesBallesRestantes(20).Build(),
        new ChasseurBuilder().Nommé("Bernard").AyantDesBallesRestantes(8).Build(),
        new ChasseurBuilder().Nommé("Robert").AyantDesBallesRestantes(12).AyantDesGalinettes(2).Build(),
    };

    private Terrain _terrain = new TerrainBuilder().AvecGalinettes(3).Build();
    private PartieStatus _status = PartieStatus.EnCours;
    private List<Event> _events = new();

    public PartieDeChasseBuilder AyantLId(Guid id)
    {
        _id = id;
        return this;
    }

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

    public PartieDeChasseBuilder QuiEstTerminee()
    {
        _status = PartieStatus.Terminée;
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
        return new PartieDeChasse(
            id: _id,
            chasseurs: _chasseurs,
            terrain: _terrain,
            status: _status,
            events: _events);
    }
}
