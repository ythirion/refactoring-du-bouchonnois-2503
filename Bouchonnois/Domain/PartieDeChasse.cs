using Bouchonnois.Domain.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    private readonly GroupeDeChasseurs _groupeDeChasseurs;
    public PartieDeChasse(Guid id, PartieStatus status, List<Chasseur>? getChasseurs, Terrain terrain, List<Event> events)
    {
        Id = id;
        Status = status;
        _groupeDeChasseurs = new GroupeDeChasseurs(getChasseurs);
        Terrain = terrain;
        Events = events;
    }

    public Guid Id { get; }

    public void AddChasseur(Chasseur chasseur) => _groupeDeChasseurs.Add(chasseur);

    public Terrain Terrain { get; }

    public PartieStatus Status { get; private set; }

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

        return _groupeDeChasseurs
            .GetVainqueurs()
            .TapError(brocouille => Events.Add(new Event(eventTime, brocouille.GetEventMessage())))
            .Tap(vainqueurs => Events.Add(new Event(eventTime, vainqueurs.GetVictoryEventMessage())))
            .Finally(result => result.IsSuccess
                ? Result.Success<string, Error>(result.Value.VainqueursNames())
                : Result.Success<string, Error>(result.Error.ToString()));
    }
    public Result<Chasseur, Error> GetTireur(string chasseur)
    {
        var tireur = _groupeDeChasseurs.GetChasseurWithName(chasseur);
        return tireur is null
            ? Result.Failure<Chasseur, Error>(new Error(DomainErrorMessages.LeChasseurNestPasDansLaPartie))
            : Result.Success<Chasseur, Error>(tireur);
    }

    public bool EstSansChasseur() => _groupeDeChasseurs.Empty();
    public string ChasseursToString()
        => string.Join(
            ", ",
            _groupeDeChasseurs
                .Get()
                .Select(c => c.Nom + $" ({c.BallesRestantes} balles)")
        );
}
