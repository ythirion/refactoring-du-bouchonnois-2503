using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class PartieDeChasseBuilder
{
    private List<Chasseur> _chasseurs = [];
    private List<Event> _events = [];
    private Guid _id = Guid.NewGuid();
    private PartieStatus _status = PartieStatus.EnCours;
    private TerrainBuilder _terrainBuilder = new();

    public static PartieDeChasseBuilder UnePartieDeChasse() => new();

    public PartieDeChasseBuilder IdentifiéePar(Guid id)
    {
        _id = id;
        return this;
    }

    public PartieDeChasseBuilder EnCours()
    {
        _status = PartieStatus.EnCours;
        return this;
    }

    public PartieDeChasseBuilder ALApéro()
    {
        _status = PartieStatus.Apéro;
        return this;
    }

    public PartieDeChasseBuilder Terminée()
    {
        _status = PartieStatus.Terminée;
        return this;
    }

    public PartieDeChasseBuilder Avec(params List<Chasseur> chasseurs)
    {
        _chasseurs = chasseurs;
        return this;
    }

    public PartieDeChasseBuilder Sur(TerrainBuilder terrainBuilder)
    {
        _terrainBuilder = terrainBuilder;
        return this;
    }

    public PartieDeChasseBuilder SurUnTerrainAyantGalinettes(int nbGalinettes)
    {
        _terrainBuilder = _terrainBuilder.AyantGalinettes(nbGalinettes);
        return this;
    }

    public PartieDeChasseBuilder SurUnTerrainRicheEnGalinettes()
    {
        _terrainBuilder = _terrainBuilder.RicheEnGalinettes();
        return this;
    }

    public PartieDeChasseBuilder SurUnTerrainSansGalinettes()
    {
        _terrainBuilder = _terrainBuilder.SansGalinettes();
        return this;
    }

    public PartieDeChasseBuilder AvecEvenements(params List<Event> events)
    {
        _events = events;
        return this;
    }

    public PartieDeChasse Build()
        => new(
            _id,
            chasseurs: _chasseurs,
            terrain: _terrainBuilder.Build(),
            status: _status,
            events: _events);
}
