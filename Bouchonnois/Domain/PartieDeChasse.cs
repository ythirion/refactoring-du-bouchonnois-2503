using Bouchonnois.UseCases.Commands;
using CSharpFunctionalExtensions;
using static Bouchonnois.Domain.Errors;

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

    public Result<PartieDeChasse, Error> PrendreLApero(Func<DateTime> timeProvider)
    {
        if (Status == PartieStatus.Apéro)
        {
            return OnEstDéjàEnTrainDePrendreLapéro();
        }

        if (Status == PartieStatus.Terminée)
        {
            return OnNePrendPasLapéroQuandLaPartieEstTerminée();
        }

        Status = PartieStatus.Apéro;
        Events.Add(new Event(timeProvider(), "Petit apéro"));

        return this;
    }
}