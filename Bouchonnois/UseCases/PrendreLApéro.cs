using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases;

public static class PrendreLApéro
{
    public record Request(Guid PartieDeChasseId) : IRequest;

    public class UseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
        : IUseCase<Request, UnitResult<Error>>
    {
        public UnitResult<Error> Handle(Request request)
            => repository
                .FindById(request.PartieDeChasseId)
                .Bind(partieDeChasse => partieDeChasse.PrendreLApéro(timeProvider))
                .Tap(repository.Save);
    }
}