using CSharpFunctionalExtensions;

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

    public UnitResult<Error> PasserAlApéro(DateTime now)
    {
        if (Status == PartieStatus.Apéro)
        {
            return UnitResult.Failure(new Error("On est déjà en train de prendre l'apéro"));
        }

        if (Status == PartieStatus.Terminée)
        {
            return UnitResult.Failure(new Error("On ne prend pas l'apéro quand la partie est terminée"));
        }

        Status = PartieStatus.Apéro;

        Events.Add(new Event(now, "Petit apéro"));

        return UnitResult.Success<Error>();
    }
}
