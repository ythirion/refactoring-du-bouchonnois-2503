using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class TerminerLaPartieDeChasseUseCase
{
    private readonly IPartieDeChasseRepository _repository;
    private readonly Func<DateTime> _timeProvider;

    public TerminerLaPartieDeChasseUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public string TerminerLaPartie(Guid id)
    {
        var partieDeChasse = _repository.GetById(id);

        var classement = partieDeChasse
            .Chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key)
            .ToList();

        if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            throw new QuandCestFiniCestFini();
        }

        partieDeChasse.Status = PartieStatus.Terminée;

        string result;

        if (classement.All(group => group.Key == 0))
        {
            result = "Brocouille";
            partieDeChasse.Events.Add(
                new Event(_timeProvider(), "La partie de chasse est terminée, vainqueur : Brocouille")
            );
        }
        else
        {
            var vainqueurs = classement[0];
            result = string.Join(", ", vainqueurs.Select(c => c.Nom));
            partieDeChasse.Events.Add(
                new Event(_timeProvider(),
                    $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                )
            );
        }


        _repository.Save(partieDeChasse);

        return result;
    }
}