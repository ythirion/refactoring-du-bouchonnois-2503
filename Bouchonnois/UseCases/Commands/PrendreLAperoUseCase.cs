using Bouchonnois.Domain;
using Bouchonnois.UseCases.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public record PrendreLAperoCommand(Guid PartieDeChasseId) : ICommand;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
    : ICommandUseCase<PrendreLAperoCommand>
{
    public UnitResult<Error> Handle(PrendreLAperoCommand command)
        => repository.GetByIdMaybe(command.PartieDeChasseId)
            .Bind(partieDeChasse => partieDeChasse.PrendreLApero(timeProvider))
            .Tap(repository.Save);
}