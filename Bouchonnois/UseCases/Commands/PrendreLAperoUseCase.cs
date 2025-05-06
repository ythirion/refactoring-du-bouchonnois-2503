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

        if (partieDeChasse == null) throw new LaPartieDeChasseNexistePas();
        if (partieDeChasse.Status == PartieStatus.Apéro) throw new OnEstDéjàEnTrainDePrendreLapéro();
        if (partieDeChasse.Status == PartieStatus.Terminée) throw new OnPrendPasLapéroQuandLaPartieEstTerminée();

        partieDeChasse.Status = PartieStatus.Apéro;
        partieDeChasse.Events.Add(new Event(timeProvider(), "Petit apéro"));
        repository.Save(partieDeChasse);
    }

    public UnitResult<Error> HandleWithoutException(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        Result<PartieDeChasse, LaPartieDeChasseNexistePas> retrievedResult = partieDeChasse.ToResult(new LaPartieDeChasseNexistePas());

        if (retrievedResult.IsSuccess)
        {
            switch (partieDeChasse.Status)
            {
                case PartieStatus.Apéro:
                    return UnitResult.Failure(new Error("On est déjà en train de prendre l'apéro"));
                case PartieStatus.Terminée:
                    return UnitResult.Failure(new Error("On ne prend pas l'apéro quand la partie est terminée"));
            }

            retrievedResult.Value.Status = PartieStatus.Apéro;

            partieDeChasse.Events.Add(new Event(timeProvider(), "Petit apéro"));
            // TODO > First save then add event?
            repository.Save(partieDeChasse);
        }

        return UnitResult.Success<Error>();
    }
}

public record Error(string Message);
