using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TirerSurGalinetteUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id, string chasseur)
        => repository
            .GetSafeById(id)
            .ToResult(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas))
            .Bind(p => p
                .ChasseurTireSurUneGalinette(chasseur,
                    timeProvider(),
                    p => repository.Save(p))
                .Tap(() => repository.Save(p)));
}
