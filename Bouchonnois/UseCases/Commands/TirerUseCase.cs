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
            .Bind(partie => SiLaPartiePermetDeTirer(chasseur, partie))
            .Bind(partie => RécupèreLeChasseur(chasseur, partie))
            .Bind(result => Tire(chasseur, result));

    private UnitResult<Error> Tire(
        string chasseur,
        (PartieDeChasse PartieDeChasse, Chasseur Chasseur) result)
    {
        var chasseurTire = result.Chasseur
            .TireSansCible()
            .Tap(() =>
            {
                result.PartieDeChasse.Events.Add(new Event(timeProvider(), $"{chasseur} tire"));
                repository.Save(result.PartieDeChasse);
            });
        if (chasseurTire.IsSuccess)
        {
            return UnitResult.Success<Error>();
        }

        result.PartieDeChasse.Events.Add(new Event(timeProvider(),
            $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main"));

        repository.Save(result.PartieDeChasse);

        return UnitResult
            .Failure(new Error(DomainErrorMessages.TasPlusDeBallesMonVieuxChasseALaMain));
    }

    private static Result<(PartieDeChasse PartieDeChasse, Chasseur Chasseur), Error> RécupèreLeChasseur(string chasseur, PartieDeChasse p)
    {
        return p
            .GetChasseur(chasseur)
            .Map(c => (PartieDeChasse: p, Chasseur: c));
    }
    private Result<PartieDeChasse, Error> SiLaPartiePermetDeTirer(string chasseur, PartieDeChasse p)
    {
        return p.PeutTirer(chasseur, timeProvider())
            .TapError(() => repository.Save(p))
            .Map(() => p);
    }
}
