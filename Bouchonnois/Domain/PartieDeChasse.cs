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

    private void Emet(ApéroDémarré @event) => Events.Add(@event);

    private bool LaPartieEstTerminée() => Status == PartieStatus.Terminée;

    private bool LApéroEstEnCours() => Status == PartieStatus.Apéro;

    public Result<PartieDeChasse, Error> TirerSurGalinette(
        string chasseur,
        IPartieDeChasseRepository partieDeChasseRepository,
        Func<DateTime> timeProvider)
    {
        if (Terrain.NbGalinettes == 0) return Errors.TasTropPicoléMonVieuxTasRienTouché();

        var chasseurQuiTire = Chasseurs.FirstOrDefault(c => c.Nom == chasseur);
        if (chasseurQuiTire is null) return Errors.ChasseurInconnu(chasseur);

        if (Status == PartieStatus.Apéro)
        {
            Events.Add(
                new Event(
                    timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
            partieDeChasseRepository.Save(this);
            return Errors.OnTirePasPendantLapéroCestSacré();
        }

        if (Status == PartieStatus.Terminée)
        {
            Events.Add(
                new Event(
                    timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
            partieDeChasseRepository.Save(this);

            return Errors.OnTirePasQuandLaPartieEstTerminée();
        }


        if (chasseurQuiTire.BallesRestantes == 0)
        {
            Events.Add(
                new Event(
                    timeProvider(),
                    $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
            partieDeChasseRepository.Save(this);

            return Errors.TasPlusDeBallesMonVieuxChasseALaMain();
        }

        chasseurQuiTire.BallesRestantes--;
        chasseurQuiTire.NbGalinettes++;
        Terrain.NbGalinettes--;
        Events.Add(new Event(timeProvider(), $"{chasseur} tire sur une galinette"));

        partieDeChasseRepository.Save(this);

        return this;
    }
}