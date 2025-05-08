using Bouchonnois.Domain;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
        => repository
            .GetById(id)
            .ToResult(UseCasesErrorMessages.LaPartieDeChasseNExistePas())
            .Bind(p => p.PasserAlApéro(timeProvider()).Map(() => p))
            .Tap(p => repository.Save(p));
}
