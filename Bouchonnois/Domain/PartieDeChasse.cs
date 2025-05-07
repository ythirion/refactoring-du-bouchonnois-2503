using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;
using static Bouchonnois.Domain.Common.Errors;
using static CSharpFunctionalExtensions.UnitResult;

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

    public Result<PartieDeChasse, Error> PrendreLApéro(Func<DateTime> timeProvider)
    {
        if (LApéroEstEnCours()) return OnEstDéjàEnTrainDePrendreLApéro();

        if (LaPartieEstTerminée()) return OnNePrendPasLapéroQuandLaPartieEstTerminée();

        Status = PartieStatus.Apéro;

        Emet(new ApéroDémarré(timeProvider()));

        return this;
    }

    public (PartieDeChasse, Maybe<Error>) TirerSurUneGalinette(string chasseur, Func<DateTime> timeProvider)
    {
        var (_, _, _, error) = Success<Error>()
            .Bind(() => SAssurerQueLApéroEstEnCours().TapError(_ => Emet(new ChasseurAVouluTiréPendantLApéro(timeProvider(), chasseur))))
            .Bind(() => SAssurerQueLaPartieEstTerminée().TapError(_ => Emet(new ChasseurAVouluTiréQuandPartieTerminée(timeProvider(), chasseur))))
            .Bind(() => RetrieveChasseur(chasseur)
                .Bind(c => c.TireSurUneGalinette().TapError(_ => Emet(new ChasseurSansBallesAVouluTiré(timeProvider(), chasseur))))
                .Bind(_ => Terrain.ChasseurTueUneGalinette()))
            .Tap(_ => Emet(new ChasseurATiréSurUneGalinette(timeProvider(), chasseur)));

        return (this, error);

        UnitResult<Error> SAssurerQueLApéroEstEnCours()
            => FailureIf(LApéroEstEnCours, OnTirePasPendantLapéroCestSacré());

        UnitResult<Error> SAssurerQueLaPartieEstTerminée()
            => FailureIf(LaPartieEstTerminée, OnTirePasQuandLaPartieEstTerminée());
    }

    private Result<Chasseur, Error> RetrieveChasseur(string chasseur)
    {
        var result = Chasseurs.Find(c => c.Nom == chasseur);
        return result is null ? ChasseurInconnu(chasseur) : result;
    }

    private void Emet(Event @event) => Events.Add(@event);

    private bool LaPartieEstTerminée() => Status == PartieStatus.Terminée;

    private bool LApéroEstEnCours() => Status == PartieStatus.Apéro;
}