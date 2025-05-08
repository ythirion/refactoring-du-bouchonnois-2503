namespace Bouchonnois.Domain;

public class Event(DateTime date, string message)
{
    public override string ToString() => $"{Date:HH:mm} - {message}";

    public DateTime Date { get; } = date;
}

public class PetitAperoEvent(DateTime date) : Event(date, "Petit apéro");

public class RepriseDeLaChasseEvent(DateTime date) : Event(date, "Reprise de la chasse");

public class TireEchouePendantLAperoEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");

public class TireEchoueCarPartieTerminéeEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} veut tirer -> On tire pas quand la partie est terminée");

public class TireEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} tire");

public class TireEchoueSansBalleEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main");

public class TireSurUneGalinetteEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} tire sur une galinette");

public class TireSurUneGalinetteEchoueSansBalleEvent(DateTime date, string chasseur)
    : Event(date, $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main");

public class PartieDechasseDemarreEvent(DateTime date, PartieDeChasse partieDeChasse)
    : Event(date, $"La partie de chasse commence à {partieDeChasse.Terrain.Nom} avec {partieDeChasse.Chasseurs()}");

public class TermineBrocouilleEvent(DateTime date) : Event(date, "La partie de chasse est terminée, vainqueur : Brocouille");

public class TermineAvecDesVainqueursEvent(DateTime date, GroupeDeVainqueurs vainqueurs)
    : Event(date, $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}");
