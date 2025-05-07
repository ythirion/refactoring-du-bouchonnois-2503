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
                .Map(partieDeChasse => partieDeChasse.TirerSurUneGalinette(request.Chasseur, timeProvider))
                .Tap(result => repository.Save(result.Item1))
                .Bind(result => result.Item2.ToUnitResult());
    }
}