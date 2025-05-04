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

        return PrendreLApero(partieDeChasse)
            .Tap(() => repository.Save(partieDeChasse));
    }

    private UnitResult<Error> PrendreLApero(PartieDeChasse partieDeChasse)
    {
        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            return UnitResult.Failure(new Error("On est déjà en train de prendre l'apéro"));
        }

        if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            return UnitResult.Failure(new Error("On ne prend pas l'apéro quand la partie est terminée"));
        }

        partieDeChasse.Status = PartieStatus.Apéro;
        partieDeChasse.Events.Add(new Event(timeProvider(), "Petit apéro"));

        return UnitResult.Success<Error>();
    }
}

public record Error(string Message);
