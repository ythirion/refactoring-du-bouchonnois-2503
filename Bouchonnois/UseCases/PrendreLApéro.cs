using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases;

public static class PrendreLApéro
{
    public record Command(Guid PartieDeChasseId) : ICommand;

    public class UseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider) : ICommandUseCase<Command>
    {
        public UnitResult<Error> Handle(Command command)
            => repository
                .FindById(command.PartieDeChasseId)
                .Bind(partieDeChasse => partieDeChasse.PrendreLApéro(timeProvider))
                .Tap(repository.Save);
    }
}