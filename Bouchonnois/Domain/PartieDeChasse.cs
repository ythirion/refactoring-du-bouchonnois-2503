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

    public UnitResult<Error> PasserAlApéro(DateTime eventTime)
    {
        return Status switch
        {
            PartieStatus.Apéro => UnitResult.Failure(new Error(DomainErrorMessages.OnEstDéjàEnTrainDePrendreLApéro)),
            PartieStatus.Terminée => UnitResult.Failure(new Error(DomainErrorMessages.OnNePrendPasLApéroQuandLaPartieEstTerminée)),
            _ => AlApero(eventTime)
        };

        UnitResult<Error> AlApero(DateTime now)
        {
            Status = PartieStatus.Apéro;
            Events.Add(new Event(now, "Petit apéro"));

            return UnitResult.Success<Error>();
        }
    }
    public UnitResult<Error> ReprendreLaPartie(DateTime eventTime)
    {
        return Status switch
        {
            PartieStatus.EnCours => UnitResult.Failure(new Error(DomainErrorMessages.LaPartieDeChasseEstDejaEnCours)),
            PartieStatus.Terminée => UnitResult.Failure(new Error(DomainErrorMessages.QuandCestFiniCestFini)),
            _ => Reprend(eventTime)
        };

        UnitResult<Error> Reprend(DateTime now)
        {
            Status = PartieStatus.EnCours;
            Events.Add(new Event(now, "Reprise de la chasse"));

            return UnitResult.Success<Error>();
        }
    }
    public Result<string, Error> TerminerLaPartie(DateTime eventTime)
    {
        if (Status == PartieStatus.Terminée)
        {
            return Result.Failure<string, Error>(new Error(DomainErrorMessages.QuandCestFiniCestFini));
        }

        Status = PartieStatus.Terminée;

        string meilleurChasseurs;

        var classement = Chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key)
            .ToList();

        if (classement.All(group => group.Key == 0))
        {
            Events.Add(
                new Event(eventTime, "La partie de chasse est terminée, vainqueur : Brocouille")
            );
        }
        else
        {
            var vainqueurs = classement[0];
            Events.Add(
                new Event(eventTime,
                    $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                )
            );
        }

        if (classement.All(group => group.Key == 0))
        {
            meilleurChasseurs = "Brocouille";
        }
        else
        {
            var vainqueurs = classement[0];
            meilleurChasseurs = string.Join(", ", vainqueurs.Select(c => c.Nom));
        }


        return meilleurChasseurs;
    }
}
