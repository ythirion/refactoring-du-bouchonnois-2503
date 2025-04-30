using Bouchonnois.Domain;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        var potentialPartieDeChasse = repository.GetByIdMaybe(id);
        if (potentialPartieDeChasse.IsFailure)
        {
            return potentialPartieDeChasse.Error;
        }
        
        var partieDeChasse = potentialPartieDeChasse.Value;

        var result = partieDeChasse.PrendreLApero(timeProvider);
        if (result.IsFailure)
        {
            return result.Error;
        }
        
        repository.Save(partieDeChasse);

        return UnitResult.Success<Error>();
    }
}