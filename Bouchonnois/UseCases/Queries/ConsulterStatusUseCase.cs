using Bouchonnois.Domain;
using Bouchonnois.UseCases.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Queries;

public class ConsulterStatusUseCase(IPartieDeChasseRepository repository)
{
    public Result<string, Error> Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse.HasNoValue)
        {
            return new Error(UseCasesErrorMessages.LaPartieDeChasseNExistePas);
        }

        return string.Join(
            Environment.NewLine,
            partieDeChasse
                .Value
                .Events
                .OrderByDescending(@event => @event.Date)
                .Select(@event => @event.ToString())
        );
    }
}
