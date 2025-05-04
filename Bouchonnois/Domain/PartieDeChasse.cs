using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Commands;

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

    public Terrain Terrain { get; }

    public PartieStatus Status { get; private set; }

    public List<Event> Events { get; }

    public void AddChasseur(Chasseur chasseur) => _groupeDeChasseurs.Add(chasseur);
    public Result<Chasseur, Error> GetChasseur(string chasseur)
        => _groupeDeChasseurs.GetChasseurWithName(chasseur)
            .ToResult(new Error(DomainErrorMessages.LeChasseurNestPasDansLaPartie));

    public bool EstSansChasseur() => _groupeDeChasseurs.Empty();

    public string ChasseursToString()
        => string.Join(
            ", ",
            _groupeDeChasseurs
                .Get()
                .Select(c => c.Nom + $" ({c.BallesRestantes} balles)")
        );

    public UnitResult<Error> PasserAlApéro(DateTime eventTime)
    {
        return Status switch
        {
            PartieStatus.Apéro => new Error(DomainErrorMessages.OnEstDéjàEnTrainDePrendreLApéro),
            PartieStatus.Terminée => new Error(DomainErrorMessages.OnNePrendPasLApéroQuandLaPartieEstTerminée),
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

    public UnitResult<Error> PeutTirer(string chasseur, DateTime eventTime)
    {
        if (Status == PartieStatus.Apéro)
        {
            Events.Add(new Event(eventTime,
                $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
            return new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré);
        }

        if (Status == PartieStatus.Terminée)
        {
            Events.Add(new Event(eventTime,
                $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
            return new Error(DomainErrorMessages.OnTirePasQuandLaPartieEstTerminée);
        }

        return UnitResult.Success<Error>();
    }
    public UnitResult<Error> ChasseurTireSurUneGalinette(string chasseur, DateTime timeProvider, Action<PartieDeChasse> action)
    {
        var assezDeGalinettes = AAssezDeGalinettes();
        if (assezDeGalinettes.IsFailure)
        {
            return assezDeGalinettes;
        }
        var laPartieEstEncours = PartieEnCours(chasseur, timeProvider, action);
        if (laPartieEstEncours.IsFailure)
        {
            return laPartieEstEncours;
        }

        var chasseurQuiVeutTirer = GetChasseur(chasseur);
        if (chasseurQuiVeutTirer.IsFailure)
        {
            return chasseurQuiVeutTirer;
        }

        var chasseurQuiTire = chasseurQuiVeutTirer.Value;

        return chasseurQuiTire
            .TireSurUneGalinette()
            .Tap(() =>
            {
                Terrain.NbGalinettes--;
                Events.Add(new Event(timeProvider, $"{chasseur} tire sur une galinette"));
            })
            .TapError(error =>
            {
                Events.Add(new Event(timeProvider,
                    $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
                action(this);
            });
    }

    private UnitResult<Error> PartieEnCours(
        string chasseur,
        DateTime timeProvider,
        Action<PartieDeChasse> action)
    {
        if (Status == PartieStatus.Apéro)
        {
            Events.Add(
                new Event(
                    timeProvider,
                    $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
            action(this);

            return new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré);
        }

        if (Status == PartieStatus.Terminée)
        {
            Events.Add(
                new Event(
                    timeProvider,
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
            action(this);

            return new Error(DomainErrorMessages.OnTirePasQuandLaPartieEstTerminée);
        }
        return UnitResult.Success<Error>();
    }

    private UnitResult<Error> AAssezDeGalinettes()
        => Terrain.NbGalinettes == 0
            ? new Error(DomainErrorMessages.TasTropPicoléMonVieuxTasRienTouché)
            : UnitResult.Success<Error>();
}
