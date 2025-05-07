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

    public Result<PartieDeChasse, (Error, Maybe<PartieDeChasse>)> TirerSurGalinette(
        string chasseur,
        Func<DateTime> timeProvider)
    {
        var result2 = Terrain.ChasseurTueUneGalinette();
        if (result2.IsFailure) return (result2.Error, Maybe<PartieDeChasse>.None);

        if (LApéroEstEnCours())
        {
            Emet(new ChasseurAVouluTiréPendantLApéro(timeProvider(), chasseur));
            return (OnTirePasPendantLapéroCestSacré(), this);
        }

        if (LaPartieEstTerminée())
        {
            Emet(new ChasseurAVouluTiréQuandPartieTerminée(timeProvider(), chasseur));
            return (OnTirePasQuandLaPartieEstTerminée(), this);
        }

        var chasseurQuiTire = RetrieveChasseur(chasseur);
        if (chasseurQuiTire.IsFailure) return (chasseurQuiTire.Error, Maybe<PartieDeChasse>.None);
       
        var result = chasseurQuiTire
            .Bind(c => c.TireSurUneGalinette())
            .TapError(_ => Emet(new ChasseurSansBallesAVouluTiré(timeProvider(), chasseur)));
        if (result.IsFailure) return (result.Error, this);

        Emet(new ChasseurATiréSurUneGalinette(timeProvider(), chasseur));

        return this;
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