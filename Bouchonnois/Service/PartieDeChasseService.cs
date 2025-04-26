using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Service;

public class PartieDeChasseService
{
    private readonly IPartieDeChasseRepository _repository;
    private readonly Func<DateTime> _timeProvider;

    public PartieDeChasseService(
        IPartieDeChasseRepository repository,
        Func<DateTime> timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
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
