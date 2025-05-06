using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class ReprendreLaPartieUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public void Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        if (partieDeChasse.Status == PartieStatus.EnCours)
        {
            throw new LaChasseEstDéjàEnCours();
        }

        if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            throw new QuandCestFiniCestFini();
        }

        partieDeChasse.Status = PartieStatus.EnCours;
        partieDeChasse.Events.Add(new Event(timeProvider(), "Reprise de la chasse"));
        repository.Save(partieDeChasse);
    }
}
