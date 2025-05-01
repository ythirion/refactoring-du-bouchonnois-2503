using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

using TimeProvider = System.Func<System.DateTime>;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, TimeProvider timeProvider)
{
    public UnitResult<Error> Handle(PrendreLapéro prendreLapéro)
        => repository.RetrieveById(prendreLapéro.Id)
            .ToResult(new Error("La partie de chasse n'existe pas"))
            .Bind(Handle);

    private UnitResult<Error> Handle(PartieDeChasse partieDeChasse)
        => partieDeChasse.PrendreLapéro(timeProvider)
            .Tap(() => repository.Save(partieDeChasse));
}
