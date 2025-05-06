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
        {
            var id = request.Id;
            var chasseur = request.Chasseur;
            var partieDeChasse = repository.GetById(id);

            if (partieDeChasse == null) return Errors.LaPartieDeChasseNexistePas();

            return partieDeChasse.TirerSurGalinette(chasseur, repository, timeProvider);
        }
    }
}