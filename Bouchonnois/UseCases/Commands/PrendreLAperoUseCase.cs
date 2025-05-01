using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

using TimeProvider = System.Func<System.DateTime>;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, TimeProvider timeProvider)
{
    public UnitResult<Error> Handle(Guid id) => HandleCommand(new PrendreLapéro(id));

    public UnitResult<Error> HandleCommand(PrendreLapéro prendreLapéro)
    {
        var partieDeChasse = repository.GetById(prendreLapéro.Id);
        if (partieDeChasse == null)
        {
            return new Error("La partie de chasse n'existe pas");
        }

        return partieDeChasse.PrendreLapéro(timeProvider)
            .Tap(() => repository.Save(partieDeChasse));
    }
}
