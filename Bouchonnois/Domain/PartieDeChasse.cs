using Bouchonnois.Domain.Errors;

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
            Emet(new PetitAperoEvent(now));

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
            Emet(new RepriseDeLaChasseEvent(now));

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
            .TapError(brocouille => Emet(new TermineBrocouilleEvent(eventTime)))
            .Tap(vainqueurs => Emet(new TermineAvecDesVainqueursEvent(eventTime, vainqueurs)))
            .Finally(result => result.IsSuccess
                ? result.Value.VainqueursNames()
                : result.Error.ToString());
    }

    public UnitResult<Error> ChasseurTireSurUneGalinette(string nom, DateTime timeProvider, Action<PartieDeChasse> action)
        => AvecDeGalinettes()
            .Ensure(() => EstEnCours(nom, timeProvider, action))
            .Bind(() => GetChasseur(nom))
            .Bind(chasseur => TireSurLaGalinette(nom, timeProvider, action, chasseur));

    private UnitResult<Error> TireSurLaGalinette(string nom, DateTime timeProvider, Action<PartieDeChasse> action, Chasseur chasseur)
        => chasseur
            .TireSurUneGalinette()
            .Tap(() => Terrain.UneGalinetteEnMoins())
            .Tap(() => { Emet(new TireSurUneGalinetteEvent(timeProvider, nom)); })
            .TapError(_ =>
            {
                Emet(new TireSurUneGalinetteEchoueSansBalleEvent(timeProvider, nom));
                action(this);
            });

    public UnitResult<Error> EstEnCours(string chasseur,
        DateTime timeProvider,
        Action<PartieDeChasse> action)
    {
        switch (Status)
        {
            case PartieStatus.Apéro:
                Emet(new TireEchouePendantLAperoEvent(timeProvider, chasseur));
                action(this);

                return new Error(DomainErrorMessages.OnTirePasPendantLapéroCestSacré);
            case PartieStatus.Terminée:
                Emet(new TireEchoueCarPartieTerminéeEvent(timeProvider, chasseur));
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

    public void Emet(Event @event) => Events.Add(@event);
}
