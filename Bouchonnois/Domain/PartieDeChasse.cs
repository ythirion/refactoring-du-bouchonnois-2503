using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;
using static Bouchonnois.Domain.Common.Errors;

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

    public (PartieDeChasse, Maybe<Error>) TirerSurGalinette(
        string chasseur,
        Func<DateTime> timeProvider)
    {
        var result2 = Terrain.ChasseurTueUneGalinette();
        if (result2.IsFailure) return (this, result2.Error);

        if (LApéroEstEnCours())
        {
            Emet(new ChasseurAVouluTiréPendantLApéro(timeProvider(), chasseur));
            return (this, OnTirePasPendantLapéroCestSacré());
        }

        if (LaPartieEstTerminée())
        {
            Emet(new ChasseurAVouluTiréQuandPartieTerminée(timeProvider(), chasseur));
            return (this, OnTirePasQuandLaPartieEstTerminée());
        }

        var chasseurQuiTire = RetrieveChasseur(chasseur);
        if (chasseurQuiTire.IsFailure) return (this, chasseurQuiTire.Error);

        var result = chasseurQuiTire
            .Bind(c => c.TireSurUneGalinette())
            .TapError(_ => Emet(new ChasseurSansBallesAVouluTiré(timeProvider(), chasseur)));
        if (result.IsFailure) return (this, result.Error);

        Emet(new ChasseurATiréSurUneGalinette(timeProvider(), chasseur));

        return (this, Maybe<Error>.None);
    }

    private Result<Chasseur, Error> RetrieveChasseur(string chasseur)
    {
        var result = Chasseurs.Find(c => c.Nom == chasseur);
        return result is null ? ChasseurInconnu(chasseur) : result;
    }

    private void Emet(Event @event) => Events.Add(@event);

    private bool LaPartieEstTerminée() => Status == PartieStatus.Terminée;

    private bool LApéroEstEnCours() => Status == PartieStatus.Apéro;

    private bool TerrainSansGalinettes() => Terrain.NbGalinettes == 0;
}