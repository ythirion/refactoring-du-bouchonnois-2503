using System.Collections.ObjectModel;

namespace Bouchonnois.Domain;

public class GroupeDeVainqueurs(List<Chasseur> vainqueurs) : Collection<Chasseur>(vainqueurs);

public static class GroupeDeVainqueursExtensions
{
    public static string VainqueursNames(this GroupeDeVainqueurs vainqueurs)
        => string.Join(", ", vainqueurs.Select(c => c.Nom));
}
