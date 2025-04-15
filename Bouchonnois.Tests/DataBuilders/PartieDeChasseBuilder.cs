using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.DataBuilders;

public class PartieDeChasseBuilder
{
    private readonly Guid _id = Guid.NewGuid();

    private List<Chasseur> _chasseurs = new List<Chasseur>
    {
        new ChasseurBuilder().Nommé("Dédé").AyantDesBallesRestantes(20).Build(),
        new ChasseurBuilder().Nommé("Bernard").AyantDesBallesRestantes(8).Build(),
        new ChasseurBuilder().Nommé("Robert").AyantDesBallesRestantes(12).AyantDesGalinettes(2).Build(),
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

public class ChasseurBuilder
{
    private string _nom = "Chasseur inconnu";
    private int _ballesRestantes;
    private int _nbGalinettes;

    public ChasseurBuilder Nommé(string nom)
    {
        _nom = nom;
        return this;
    }

    public ChasseurBuilder AyantDesBallesRestantes(int ballesRestantes)
    {
        _ballesRestantes = ballesRestantes;
        return this;
    }

    public ChasseurBuilder AyantDesGalinettes(int galinettes)
    {
        _nbGalinettes = galinettes;
        return this;
    }

    public Chasseur Build()
    {
        return new Chasseur(
            nom: _nom,
            ballesRestantes: _ballesRestantes,
            nbGalinettes: _nbGalinettes);
    }
}
