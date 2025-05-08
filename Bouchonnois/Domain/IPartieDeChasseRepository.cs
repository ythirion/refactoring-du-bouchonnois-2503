using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public interface IPartieDeChasseRepository
{
    void Save(PartieDeChasse partieDeChasse);

    Maybe<PartieDeChasse> GetById(Guid partieDeChasseId);
}
