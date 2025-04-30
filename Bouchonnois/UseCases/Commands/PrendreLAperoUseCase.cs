using Bouchonnois.Domain;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
        => repository.GetByIdMaybe(id)
            .Map(partieDeChasse => partieDeChasse.PrendreLApero(timeProvider))
            .Ensure(result => result.IsSuccess, result => result.Error)
            .Tap(result => result.Tap(repository.Save));
}