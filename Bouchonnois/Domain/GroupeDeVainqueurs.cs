using System.Collections.ObjectModel;

namespace Bouchonnois.Domain;

internal class GroupeDeVainqueurs(List<Chasseur> vainqueurs) : Collection<Chasseur>(vainqueurs)
{
    public string GetVictoryEventMessage()
        => $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}";

    public string VainqueursNames() => string.Join(", ", vainqueurs.Select(c => c.Nom));
}
