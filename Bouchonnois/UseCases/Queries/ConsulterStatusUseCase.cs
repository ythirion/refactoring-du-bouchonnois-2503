using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases.Queries;

public class ConsulterStatusUseCase(IPartieDeChasseRepository repository)
{
    public string ConsulterStatus(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

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
