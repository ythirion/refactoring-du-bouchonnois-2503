using System.Runtime.InteropServices.JavaScript;

using Bouchonnois.Domain;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class PrendreLAperoUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public UnitResult<Error> Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);
        if (partieDeChasse == null)
        {
            return new Error("La partie de chasse n'existe pas");
        }

        if (partieDeChasse.Status == PartieStatus.Apéro)
        {
            return new Error("On est déjà en train de prendre l'apéro");
        }

        if (partieDeChasse.Status == PartieStatus.Terminée)
        {
            return new Error("On ne prend pas l'apéro quand la partie est terminée");
        }

        partieDeChasse.Status = PartieStatus.Apéro;
        partieDeChasse.Events.Add(new Event(timeProvider(), "Petit apéro"));
        repository.Save(partieDeChasse);
        
        return UnitResult.Success<Error>();
    }
}

public record Error(string Message);
