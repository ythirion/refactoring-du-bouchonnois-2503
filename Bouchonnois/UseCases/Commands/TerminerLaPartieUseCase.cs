using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases.Commands;

public class TerminerLaPartieUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public string Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

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
                new Event(timeProvider(), "La partie de chasse est terminée, vainqueur : Brocouille")
            );
        }
        else
        {
            var vainqueurs = classement[0];
            result = string.Join(", ", vainqueurs.Select(c => c.Nom));
            partieDeChasse.Events.Add(
                new Event(timeProvider(),
                    $"La partie de chasse est terminée, vainqueur : {string.Join(", ", vainqueurs.Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                )
            );
        }


        repository.Save(partieDeChasse);

        return result;
    }
}
