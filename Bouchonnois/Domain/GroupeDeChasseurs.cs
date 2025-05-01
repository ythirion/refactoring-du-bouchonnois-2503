using System.Collections.ObjectModel;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

internal class GroupeDeChasseurs(List<Chasseur>? chasseurs)
{
    private readonly List<Chasseur> _chasseurs = chasseurs ?? [];
    public void Add(Chasseur chasseur) => _chasseurs.Add(chasseur);

    public IReadOnlyCollection<Chasseur> Get()
        => new ReadOnlyCollection<Chasseur>(_chasseurs.ToArray());

    public Result<GroupeDeVainqueurs, Brocouille> GetVainqueurs()
    {
        var classement = _chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key)
            .ToList();

        if (classement.All(group => group.Key == 0))
        {
            return Result.Failure<GroupeDeVainqueurs, Brocouille>(new Brocouille());
        }

        return Result.Success<GroupeDeVainqueurs, Brocouille>(
            new GroupeDeVainqueurs(classement.First().ToList()));
    }
    public Chasseur? GetChasseurWithName(string name) => _chasseurs
        .FirstOrDefault(c => c.Nom == name);

    public bool Empty() => !_chasseurs.Any();
}
