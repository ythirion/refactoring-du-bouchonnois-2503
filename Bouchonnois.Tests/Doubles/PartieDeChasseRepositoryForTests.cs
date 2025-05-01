using Bouchonnois.Domain;

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
    public Maybe<PartieDeChasse> GetSafeById(Guid partieDeChasseId) => GetById(partieDeChasseId);

    public void Add(PartieDeChasse partieDeChasse) => _partiesDeChasse[partieDeChasse.Id] = partieDeChasse;
    public PartieDeChasse SavedPartieDeChasse() => _savedPartieDeChasse!;
}
