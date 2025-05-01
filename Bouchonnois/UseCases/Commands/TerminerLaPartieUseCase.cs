using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Errors;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TerminerLaPartieUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Result<string, Error> Handle(Guid id)
        => repository
            .GetSafeById(id)
            .ToResult(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas))
            .Bind(p => p
                .TerminerLaPartie(timeProvider())
                .Tap(() => repository.Save(p)));
}
