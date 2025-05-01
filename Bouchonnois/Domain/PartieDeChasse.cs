using Bouchonnois.UseCases.Commands;

using CSharpFunctionalExtensions;

using TimeProvider = System.Func<System.DateTime>;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    public PartieDeChasse(Guid id, PartieStatus status, List<Chasseur> chasseurs, Terrain terrain, List<Event> events)
    {
        Id = id;
        Status = status;
        Chasseurs = chasseurs;
        Terrain = terrain;
        Events = events;
    }

    public Guid Id { get; }

    public List<Chasseur> Chasseurs { get; }

    public Terrain Terrain { get; }

    public PartieStatus Status { get; set; }

    public List<Event> Events { get; }

    public UnitResult<Error> PrendreLapéro(TimeProvider timeProvider)
    {
        if (LapéroEstEnCours()) return new Error("On est déjà en train de prendre l'apéro");
        if (Terminée()) return new Error("On ne prend pas l'apéro quand la partie est terminée");

        Status = PartieStatus.Apéro;
        EmitEvent(new ApéroADémarré(Id, timeProvider()));

        return UnitResult.Success<Error>();
    }

    private bool Terminée() => Status == PartieStatus.Terminée;
    private bool LapéroEstEnCours() => Status == PartieStatus.Apéro;
    private void EmitEvent(ApéroADémarré @event) => Events.Add(@event);
}
