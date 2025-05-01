using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public void Handle(Guid id)
        => HandleWithoutException(id)
            .TapErrorIf(error => error.Message == "La partie de chasse n'existe pas",
                _ => throw new LaPartieDeChasseNexistePas())
            .TapErrorIf(error => error.Message == "On est déjà en train de prendre l'apéro",
                _ => throw new OnEstDéjàEnTrainDePrendreLapéro())
            .TapErrorIf(error => error.Message == "On ne prend pas l'apéro quand la partie est terminée",
                _ => throw new OnPrendPasLapéroQuandLaPartieEstTerminée());

    public UnitResult<Error> HandleWithoutException(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        if (partieDeChasse == null)
        {
            return new Error("La partie de chasse n'existe pas");
        }

        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            return new Error("On est déjà en train de prendre l'apéro");
        }
        else if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            return new Error("On ne prend pas l'apéro quand la partie est terminée");
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
