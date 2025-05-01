using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        return repository
            .GetSafeById(id)
            .ToResult(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas))
            .Bind(p =>
                p.PasserAlApéro(timeProvider())
                    .Tap(() => repository.Save(p))
            );
    }
}
