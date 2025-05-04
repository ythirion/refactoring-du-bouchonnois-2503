using System.Collections.ObjectModel;

namespace Bouchonnois.Domain;

public class GroupeDeVainqueurs(List<Chasseur> vainqueurs) : Collection<Chasseur>(vainqueurs)
{
}

public static class GroupeDeVainqueursExtensions
{
    public static string EventMessage(this GroupeDeVainqueurs vainqueurs)
        => $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}";

    public static string VainqueursNames(this GroupeDeVainqueurs vainqueurs)
        => string.Join(", ", vainqueurs.Select(c => c.Nom));
}
