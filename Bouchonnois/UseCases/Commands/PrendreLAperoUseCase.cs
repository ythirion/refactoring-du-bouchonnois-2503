using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

using TimeProvider = System.Func<System.DateTime>;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, TimeProvider timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        if (partieDeChasse == null)
        {
            return new Error("La partie de chasse n'existe pas");
        }

        return partieDeChasse.PrendreLapéro(timeProvider)
            .Tap(() => repository.Save(partieDeChasse));
    }
}

public record Error(string Message);
