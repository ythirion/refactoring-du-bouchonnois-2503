using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TirerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{

    public UnitResult<Error> Handle(Guid id, string chasseur)
        => repository
            .GetSafeById(id)
            .ToResult(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas))
            .Bind(p => p.PeutTirer(chasseur, timeProvider())
                .TapError(() => repository.Save(p))
                .Map(() => p))
            .Bind(p => p
                .GetChasseur(chasseur)
                .Map(c => (PartieDeChasse: p, Chasseur: c)))
            .Bind(result =>
            {
                if (result.Chasseur.BallesRestantes != 0)
                {
                    result.PartieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire"));
                    result.Chasseur.BallesRestantes--;

                    repository.Save(result.PartieDeChasse);
                    return UnitResult.Success<Error>();
                }

                result.PartieDeChasse.Events.Add(new Event(timeProvider(),
                    $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main"));

                repository.Save(result.PartieDeChasse);

                return UnitResult
                    .Failure(new Error(DomainErrorMessages.TasPlusDeBallesMonVieuxChasseALaMain));
            });
}
