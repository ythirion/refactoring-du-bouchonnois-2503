using Bouchonnois.Domain;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TerminerLaPartieUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Result<string, Error> Handle(Guid id)
        => repository
            .GetById(id)
            .ToResult(UseCasesErrorMessages.LaPartieDeChasseNExistePas())
            .Bind(p => p
                .TerminerLaPartie(timeProvider())
                .Tap(() => repository.Save(p)));
}
