using Bouchonnois.Domain;
using Bouchonnois.Domain.Errors;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public record DemarrerRequest((string nom, int nbGalinettes) TerrainDeChasse, List<(string nom, int nbBalles)> Chasseurs);

public class DemarrerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Result<Guid, Error> Handle(DemarrerRequest request)
    {
        var terrainDeChasse = new Terrain(request.TerrainDeChasse.nom, request.TerrainDeChasse.nbGalinettes);

        return AvecDesGalinettes(terrainDeChasse)
            .Bind(() => PourLesChasseurs(request.Chasseurs))
            .Bind(chasseurs => Démarrer(terrainDeChasse, chasseurs));
    }

    private Result<Guid, Error> Démarrer(Terrain terrain, List<Chasseur> chasseurs)
    {
        var partieDeChasse = new PartieDeChasse(
            Guid.NewGuid(),
            PartieStatus.EnCours,
            chasseurs,
            terrain);

        if (partieDeChasse.EstSansChasseur())
        {
            return new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansChasseur);
        }

        partieDeChasse.Emet(new PartieDechasseDemarreEvent(timeProvider(), partieDeChasse));
        repository.Save(partieDeChasse);

        return Result.Success<Guid, Error>(partieDeChasse.Id);
    }

    private static UnitResult<Error> AvecDesGalinettes(Terrain terrainDeChasse)
        => terrainDeChasse.NbGalinettes <= 0
            ? new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieSansGalinettes)
            : UnitResult.Success<Error>();

    private static Result<List<Chasseur>, Error> PourLesChasseurs(List<(string nom, int nbBalles)> chasseurs)
        => chasseurs.Any(chasseur => chasseur.nbBalles == 0)
            ? new Error(DomainErrorMessages.ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle)
            : chasseurs
                .Select(chasseur => new Chasseur(chasseur.nom, chasseur.nbBalles))
                .ToList();
}
