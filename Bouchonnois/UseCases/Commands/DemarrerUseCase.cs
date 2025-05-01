using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases.Commands;

public class DemarrerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Guid Handle((string nom, int nbGalinettes) terrainDeChasse, List<(string nom, int nbBalles)> chasseurs)
    {
        if (terrainDeChasse.nbGalinettes <= 0)
        {
            throw new ImpossibleDeDémarrerUnePartieSansGalinettes();
        }

        var partieDeChasse = new PartieDeChasse(Guid.NewGuid(), PartieStatus.EnCours,
            new List<Chasseur>(),
            new Terrain(terrainDeChasse.nom, terrainDeChasse.nbGalinettes),
            new List<Event>());

        foreach (var chasseur in chasseurs)
        {
            if (chasseur.nbBalles == 0)
            {
                throw new ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle();
            }

            partieDeChasse.AddChasseur(new Chasseur(chasseur.nom, chasseur.nbBalles));
        }

        if (partieDeChasse.EstSansChasseur())
        {
            throw new ImpossibleDeDémarrerUnePartieSansChasseur();
        }

        var chasseursToString = partieDeChasse.ChasseursToString();

        partieDeChasse.Events.Add(new Event(timeProvider(),
            $"La partie de chasse commence à {partieDeChasse.Terrain.Nom} avec {chasseursToString}")
        );

        repository.Save(partieDeChasse);

        return partieDeChasse.Id;
    }
}
