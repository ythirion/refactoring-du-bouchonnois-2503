using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Tests.UseCases;

namespace Bouchonnois.Tests.DataBuilders;

public class PartieDeChasseBuilder
{
    private PartieStatus _status = PartieStatus.EnCours;
    private List<Chasseur> _chasseurs = new();
    private List<Event> _events = new();
    //private Terrain _terrain;

    public PartieDeChasseBuilder QuiEstEnCours()
    {
        _status = PartieStatus.EnCours;
        return this;
    }
    public PartieDeChasseBuilder QuandLesChasseursSontALapero()
    {
        _status = PartieStatus.Apéro;
        return this;
    }
    public PartieDeChasseBuilder QuandLaPartieDeChasseEstTerminee()
    {
        _status = PartieStatus.Terminée;
        return this;
    }
    public PartieDeChasseBuilder AvecDesChasseurs(List<Chasseur> chasseurs)
    {
        _chasseurs = chasseurs;
        return this;
    }
    public PartieDeChasseBuilder AvecDesChasseurs()
    {
        _chasseurs =
        [
            new(Data.Dede, 20),
            new(Data.Bernard, 8),
            new(Data.Robert, 12)
        ];
        return this;
    }
    public PartieDeChasseBuilder AjouterChasseur(Chasseur chasseur)
    {
        if(_chasseurs.Contains(chasseur))
            throw new ArgumentException($"Chasseur {chasseur.Nom} existe déjà!!");
        _chasseurs.Add(chasseur);
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
            terrain: new Terrain(nom: Data.Terrain, nbGalinettes: 3),
            status: _status,
            events: _events);
    }

    public PartieDeChasseBuilder ExAequo()
    {
        _chasseurs.ForEach(chasseur => chasseur.NbGalinettes = 3);
        return this;
    }
}
