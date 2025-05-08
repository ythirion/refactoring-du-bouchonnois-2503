using Bouchonnois.Domain;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class ReprendreLaPartieUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
        => repository
            .GetById(id)
            .ToResult(UseCasesErrorMessages.LaPartieDeChasseNExistePas())
            .Bind(p => p.ReprendreLaPartie(timeProvider()).Map(() => p))
            .Tap(p => repository.Save(p));
}
