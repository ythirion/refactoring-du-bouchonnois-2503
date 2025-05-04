using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public class DemarrerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Result<Guid, Error> Handle(
        (string nom, int nbGalinettes) terrainDeChasse,
        List<(string nom, int nbBalles)> chasseurs)
    {
        if (terrainDeChasse.nbGalinettes <= 0)
        {
            return new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansGalinettes);
        }
        foreach (var chasseur in chasseurs)
        {
            if (chasseur.nbBalles == 0)
            {
                return new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle);
            }
        }

        var partieDeChasse = new PartieDeChasse(Guid.NewGuid(), PartieStatus.EnCours,
            new List<Chasseur>(),
            new Terrain(terrainDeChasse.nom, terrainDeChasse.nbGalinettes),
            new List<Event>());

        foreach (var chasseur in chasseurs)
        {
            partieDeChasse.AddChasseur(new Chasseur(chasseur.nom, chasseur.nbBalles));
        }

        if (partieDeChasse.EstSansChasseur())
        {
            return new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansChasseur);
        }

        partieDeChasse
            .Events
            .Add(new Event(timeProvider(), partieDeChasse.EventMessage()));

        repository.Save(partieDeChasse);

        return partieDeChasse.Id;
    }
}
