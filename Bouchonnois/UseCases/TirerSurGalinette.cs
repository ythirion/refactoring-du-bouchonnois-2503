using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using Bouchonnois.UseCases.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases;

public static class TirerSurGalinette
{
    public record Request(Guid Id, string Chasseur) : IRequest;

    public class UseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
        : IUseCase<Request, UnitResult<Error>>
    {
        public UnitResult<Error> Handle(Request request)
            => repository
                .FindById(request.Id)
                .Bind(partieDeChasse => partieDeChasse.TirerSurGalinette(request.Chasseur, repository, timeProvider)
                    .MapError(r => r.Item1));
    }
}