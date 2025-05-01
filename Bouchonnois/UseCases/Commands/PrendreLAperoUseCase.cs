using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

using TimeProvider = System.Func<System.DateTime>;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, TimeProvider timeProvider)
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

        return partieDeChasse.PrendreLapéro(timeProvider)
            .Tap(() => repository.Save(partieDeChasse));
    }
}

public record Error(string Message);
