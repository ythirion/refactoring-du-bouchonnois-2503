using System.Collections.ObjectModel;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public class GroupeDeChasseurs(List<Chasseur>? chasseurs)
{
    private readonly List<Chasseur> _chasseurs = chasseurs ?? [];

    public Result<GroupeDeVainqueurs, Brocouille> GetVainqueurs()
        => _chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key)
            .First() is var meilleurs && meilleurs.Key == 0
            ? PasDeVainqueurs()
            : new GroupeDeVainqueurs(meilleurs.ToList());

    public Maybe<Chasseur> GetChasseurWithName(string name) => _chasseurs
        .FirstOrDefault(c => c.Nom == name);

    private Result<GroupeDeVainqueurs, Brocouille> PasDeVainqueurs() => new Brocouille();
    internal IReadOnlyCollection<Chasseur> Get()
        => new ReadOnlyCollection<Chasseur>(_chasseurs.ToArray());
}

public static class GroupeDeChasseursExtensions
{
    public static string ChasseursToString(this GroupeDeChasseurs chasseurs)
        => string.Join(
            ", ",
            chasseurs
                .Get()
                .Select(c => c.Nom + $" ({c.BallesRestantes} balles)")
        );
}
