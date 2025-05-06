using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public interface IPartieDeChasseRepository
{
    void Save(PartieDeChasse partieDeChasse);
    PartieDeChasse GetById(Guid partieDeChasseId);
    Result<PartieDeChasse, Error> FindById(Guid partieDeChasseId);
}
