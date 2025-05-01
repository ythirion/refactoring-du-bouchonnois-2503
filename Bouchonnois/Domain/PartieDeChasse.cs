using Bouchonnois.Domain.Errors;

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
        return Status switch
        {
            PartieStatus.Apéro => UnitResult.Failure(new Error(DomainErrorMessages.OnEstDéjàEnTrainDePrendreLApéro)),
            PartieStatus.Terminée => UnitResult.Failure(new Error(DomainErrorMessages.OnNePrendPasLApéroQuandLaPartieEstTerminée)),
            _ => AlApero(now)
        };

        UnitResult<Error> AlApero(DateTime eventTime)
        {
            Status = PartieStatus.Apéro;
            Events.Add(new Event(eventTime, "Petit apéro"));

            return UnitResult.Success<Error>();
        }
    }
}
