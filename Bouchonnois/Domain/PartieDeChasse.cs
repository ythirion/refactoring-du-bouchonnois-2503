using Bouchonnois.Domain.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    private readonly GroupeDeChasseurs _groupeDeChasseurs;
    public PartieDeChasse(Guid id,
        PartieStatus status,
        List<Chasseur>? getChasseurs,
        Terrain terrain,
        List<Event> events)
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

    internal string Chasseurs() => _groupeDeChasseurs.ChasseursToString();

    public void AddChasseur(Chasseur chasseur) => _groupeDeChasseurs.Add(chasseur);
    public Result<Chasseur, Error> GetChasseur(string chasseur)
        => _groupeDeChasseurs.GetChasseurWithName(chasseur)
            .ToResult(new Error(DomainErrorMessages.LeChasseurNestPasDansLaPartie));

    public bool EstSansChasseur() => _groupeDeChasseurs.Empty();

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
            PartieStatus.EnCours => new Error(DomainErrorMessages.LaPartieDeChasseEstDejaEnCours),
            PartieStatus.Terminée => new Error(DomainErrorMessages.QuandCestFiniCestFini),
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
            return new Error(DomainErrorMessages.QuandCestFiniCestFini);
        }

        Status = PartieStatus.Terminée;

        return _groupeDeChasseurs
            .GetVainqueurs()
            .TapError(brocouille => Events.Add(new Event(eventTime, brocouille.EventMessage())))
            .Tap(vainqueurs => Events.Add(new Event(eventTime, vainqueurs.EventMessage())))
            .Finally(result => result.IsSuccess
                ? result.Value.VainqueursNames()
                : result.Error.ToString());
    }

    public UnitResult<Error> ChasseurTireSurUneGalinette(string chasseur, DateTime timeProvider, Action<PartieDeChasse> action)
        => AvecDeGalinettes()
            .Ensure(() => EstEnCours(chasseur, timeProvider, action))
            .Bind(() =>
                GetChasseur(chasseur)
                    .Bind(chasseurQuiTire =>
                        chasseurQuiTire
                            .TireSurUneGalinette()
                            .Tap(() =>
                            {
                                Terrain
                                    .UneGalinetteEnMoins()
                                    .Tap(() => Events.Add(new Event(timeProvider, $"{chasseur} tire sur une galinette")));
                            })
                            .TapError(_ =>
                            {
                                Events.Add(new Event(timeProvider,
                                    $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
                                action(this);
                            })
                    )
            );


    public UnitResult<Error> EstEnCours(string chasseur,
        DateTime timeProvider,
        Action<PartieDeChasse> action)
    {
        switch (Status)
        {
            case PartieStatus.Apéro:
                Events.Add(
                    new Event(
                        timeProvider,
                        $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
                action(this);

                return new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré);
            case PartieStatus.Terminée:
                Events.Add(
                    new Event(
                        timeProvider,
                        $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                action(this);

                return new Error(DomainErrorMessages.OnTirePasQuandLaPartieEstTerminée);
            default:
                return UnitResult.Success<Error>();
        }
    }

    private UnitResult<Error> AvecDeGalinettes()
        => Terrain.NbGalinettes == 0
            ? new Error(DomainErrorMessages.TasTropPicoléMonVieuxTasRienTouché)
            : UnitResult.Success<Error>();
}

public static class PartieDeChasseExtensions
{
    public static string EventMessage(this PartieDeChasse partieDeChasse)
        => $"La partie de chasse commence à {partieDeChasse.Terrain.Nom} avec {partieDeChasse.Chasseurs()}";
}
