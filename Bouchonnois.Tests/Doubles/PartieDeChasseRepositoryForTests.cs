using Bouchonnois.Domain;
using Bouchonnois.UseCases.Commands;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Tests.Doubles;

public class PartieDeChasseRepositoryForTests : IPartieDeChasseRepository
{
    private readonly IDictionary<Guid, PartieDeChasse> _partiesDeChasse = new Dictionary<Guid, PartieDeChasse>();
    private PartieDeChasse? _savedPartieDeChasse;

    public void Save(PartieDeChasse partieDeChasse)
    {
        _savedPartieDeChasse = partieDeChasse;
        _partiesDeChasse[partieDeChasse.Id] = partieDeChasse;
    }

    public PartieDeChasse GetById(Guid partieDeChasseId)
        => (_partiesDeChasse.ContainsKey(partieDeChasseId)
            ? _partiesDeChasse[partieDeChasseId]
            : null)!;

    public Result<PartieDeChasse, Error> FindBy(Guid partieDeChasseId)
    {
        var partieDeChasse = GetById(partieDeChasseId);
        return partieDeChasse == null ? new Error("La partie de chasse n'existe pas") : partieDeChasse;
    }

    public void Add(PartieDeChasse partieDeChasse) => _partiesDeChasse[partieDeChasse.Id] = partieDeChasse;
    public PartieDeChasse SavedPartieDeChasse() => _savedPartieDeChasse!;
}
