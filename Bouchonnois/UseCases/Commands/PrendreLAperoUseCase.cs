using System.Runtime.InteropServices.JavaScript;

using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public void Handle(Guid id)
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

    public UnitResult<Error> HandleWithoutException(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        if (partieDeChasse == null)
        {
            return UnitResult.Failure(new Error("La partie de chasse n'existe pas"));
        }

        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            return UnitResult.Failure(new Error("On est déjà en train de prendre l'apéro"));
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


        return UnitResult.Success<Error>();
    }
}

public record Error(string Message);
