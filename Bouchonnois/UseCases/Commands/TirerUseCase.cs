using Bouchonnois.Domain;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class TirerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id, string chasseur)
        => repository
            .GetSafeById(id)
            .ToResult(new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas))
            .Bind(partie => LaPartiePermetDeTirer(chasseur, partie))
            .Bind(partie => PourLeTireur(chasseur, partie))
            .Bind(context => Tire(context));

    private Result<PartieDeChasse, Error> LaPartiePermetDeTirer(string chasseur, PartieDeChasse p)
        => p.PeutTirer(chasseur, timeProvider())
            .TapError(() => repository.Save(p))
            .Map(() => p);

    private static Result<(PartieDeChasse partie, Chasseur tireur), Error> PourLeTireur(string nom, PartieDeChasse p)
        => p
            .GetChasseur(nom)
            .Map(c => (p, c));

    private UnitResult<Error> Tire((PartieDeChasse partie, Chasseur tireur) context)
        => context.tireur
            .TireSansCible()
            .Tap(() =>
            {
                context.partie.Events.Add(
                    new Event(timeProvider(), $"{context.tireur.Nom} tire"));
            })
            .TapError(_ =>
            {
                context.partie.Events.Add(
                    new Event(timeProvider(),
                        $"{context.tireur.Nom} tire -> T'as plus de balles mon vieux, chasse à la main"));
            })
            .Finally(finalResult =>
            {
                repository.Save(context.partie);
                return finalResult;
            });
}
