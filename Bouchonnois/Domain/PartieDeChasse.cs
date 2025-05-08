using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    private readonly GroupeDeChasseurs _groupeDeChasseurs;
    public PartieDeChasse(Guid id,
        PartieStatus status,
        List<Chasseur>? getChasseurs,
        Terrain terrain)
    {
        Id = id;
        Status = status;
        _groupeDeChasseurs = new GroupeDeChasseurs(getChasseurs);
        Terrain = terrain;
    }

    public Guid Id { get; }

    public Terrain Terrain { get; }

    public PartieStatus Status { get; private set; }

    public List<Event> Events { get; } = [];

    internal string Chasseurs() => _groupeDeChasseurs.ChasseursToString();

    public Result<Chasseur, Error> GetChasseur(string chasseur)
        => _groupeDeChasseurs.GetChasseurWithName(chasseur)
            .ToResult(Error.LeChasseurNestPasDansLaPartieError());

    public UnitResult<Error> PasserAlApéro(DateTime eventTime)
    {
        return Status switch
        {
            PartieStatus.Apéro => Error.OnEstDéjàEnTrainDePrendreLApéroError(),
            PartieStatus.Terminée => Error.OnNePrendPasLApéroQuandLaPartieEstTerminéeError(),
            _ => AlApero(eventTime)
        };

        UnitResult<Error> AlApero(DateTime now)
        {
            Status = PartieStatus.Apéro;
            Emet(new PetitAperoEvent(now));

            return UnitResultExtensions.Success();
        }
    }
    public UnitResult<Error> ReprendreLaPartie(DateTime eventTime)
    {
        return Status switch
        {
            PartieStatus.EnCours => Error.LaPartieDeChasseEstDejaEnCoursError(),
            PartieStatus.Terminée => Error.QuandCestFiniCestFiniError(),
            _ => Reprend(eventTime)
        };

        UnitResult<Error> Reprend(DateTime now)
        {
            Status = PartieStatus.EnCours;
            Emet(new RepriseDeLaChasseEvent(now));

            return UnitResultExtensions.Success();
        }
    }

    public Result<string, Error> TerminerLaPartie(DateTime eventTime)
    {
        if (Status == PartieStatus.Terminée)
        {
            return Error.QuandCestFiniCestFiniError();
        }

        Status = PartieStatus.Terminée;

        return _groupeDeChasseurs
            .GetVainqueurs()
            .Tap(vainqueurs => Emet(new TermineAvecDesVainqueursEvent(eventTime, vainqueurs)))
            .TapError(_ => Emet(new TermineBrocouilleEvent(eventTime)))
            .Finally(result => result.IsSuccess
                ? result.Value.VainqueursNames()
                : result.Error.ToString());
    }

    public UnitResult<Error> ChasseurTireSurUneGalinette(DateTime eventTime, string nom, Action<PartieDeChasse> action)
        => AvecDeGalinettes()
            .Ensure(() => EstEnCours(nom, eventTime, action))
            .Bind(() => GetChasseur(nom))
            .Bind(chasseur => TireSurLaGalinette(eventTime, chasseur, action));

    private UnitResult<Error> TireSurLaGalinette(DateTime eventTime, Chasseur chasseur, Action<PartieDeChasse> action)
        => chasseur
            .TireSurUneGalinette()
            .Tap(() => Terrain.UneGalinetteEnMoins())
            .Tap(() => Emet(new TireSurUneGalinetteEvent(eventTime, chasseur.Nom)))
            .TapError(_ =>
            {
                Emet(new TireSurUneGalinetteEchoueSansBalleEvent(eventTime, chasseur.Nom));
                action(this);
            });

    public UnitResult<Error> EstEnCours(string chasseur,
        DateTime eventTime,
        Action<PartieDeChasse> action)
    {
        switch (Status)
        {
            case PartieStatus.Apéro:
                Emet(new TireEchouePendantLAperoEvent(eventTime, chasseur));
                action(this);

                return Error.OnTirePasPendantLapéroCestSacréError();
            case PartieStatus.Terminée:
                Emet(new TireEchoueCarPartieTerminéeEvent(eventTime, chasseur));
                action(this);

                return Error.OnTirePasQuandLaPartieEstTerminéeError();
            default:
                return UnitResultExtensions.Success();
        }
    }

    private UnitResult<Error> AvecDeGalinettes()
        => Terrain.NbGalinettes == 0
            ? Error.TasTropPicoléMonVieuxTasRienTouchéError()
            : UnitResultExtensions.Success();

    public void Emet(Event @event) => Events.Add(@event);
}
