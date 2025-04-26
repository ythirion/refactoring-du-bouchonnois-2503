using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Service;

public class PartieDeChasseService
{
    private readonly IPartieDeChasseRepository _repository;

    public PartieDeChasseService(IPartieDeChasseRepository repository)
    {
        _repository = repository;
    }

    public string ConsulterStatus(Guid id)
    {
        var partieDeChasse = _repository.GetById(id);

        if (partieDeChasse == null)
        {
            throw new LaPartieDeChasseNexistePas();
        }

        return string.Join(
            Environment.NewLine,
            partieDeChasse.Events
                .OrderByDescending(@event => @event.Date)
                .Select(@event => @event.ToString())
        );
    }
}
