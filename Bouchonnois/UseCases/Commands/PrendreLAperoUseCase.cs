using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null)
            return UnitResult.Failure(new Error("La partie de chasse n'existe pas"));

        return partieDeChasse
            .PasserAlApéro(timeProvider())
            .Tap(() => repository.Save(partieDeChasse));
    }
}
