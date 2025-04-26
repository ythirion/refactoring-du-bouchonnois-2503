using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public void PrendreLapéro(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            throw new OnEstDéjàEnTrainDePrendreLapéro();
        }
        else if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            throw new OnPrendPasLapéroQuandLaPartieEstTerminée();
        }
        else
        {
            partieDeChasse.Status = PartieStatus.Apéro;
            partieDeChasse.Events.Add(new Event(timeProvider(), "Petit apéro"));
            repository.Save(partieDeChasse);
        }
    }
}
