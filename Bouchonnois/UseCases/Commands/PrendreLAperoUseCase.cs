using System.Runtime.InteropServices.JavaScript;
using Bouchonnois.Domain;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        if (partieDeChasse == null)
        {
            return new Error("La partie de chasse n'existe pas");
        }

        var result = partieDeChasse.PrendreLApero(timeProvider);
        if (result.IsFailure)
        {
            return result.Error;
        }
        
        repository.Save(partieDeChasse);

        return UnitResult.Success<Error>();
    }
}

public record Error(string Message);