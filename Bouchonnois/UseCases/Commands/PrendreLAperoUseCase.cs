using Bouchonnois.Domain;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public record PrendreLAperoCommand(Guid PartieDeChasseId);

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(PrendreLAperoCommand command)
        => repository.GetByIdMaybe(command.PartieDeChasseId)
            .Map(partieDeChasse => partieDeChasse.PrendreLApero(timeProvider))
            .Ensure(result => result.IsSuccess, result => result.Error)
            .Tap(result => result.Tap(repository.Save));
}