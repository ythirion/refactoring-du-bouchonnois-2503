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
            {
                var result = p.PasserAlApéro(timeProvider());
                if (result.IsSuccess)
                {
                    repository.Save(p);
                }

                return result;
            });
    }
}
