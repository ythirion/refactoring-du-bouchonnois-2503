using System.Collections.ObjectModel;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

internal class GroupeDeChasseurs
{
    private readonly List<Chasseur> _chasseurs;
    public GroupeDeChasseurs(List<Chasseur>? chasseurs)
    {
        _chasseurs = chasseurs ?? [];
    }
    public void Add(Chasseur chasseur) => _chasseurs.Add(chasseur);

    public IReadOnlyCollection<Chasseur> Get()
        => new ReadOnlyCollection<Chasseur>(_chasseurs.ToArray());

    public Result<List<Chasseur>, Brocouille> GetVainqueurs()
    {
        var classement = _chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key)
            .ToList();

        if (classement.All(group => group.Key == 0))
        {
            return Result.Failure<List<Chasseur>, Brocouille>(new Brocouille());
        }

        return Result.Success<List<Chasseur>, Brocouille>(classement
            .First()
            .ToList());
    }
}
