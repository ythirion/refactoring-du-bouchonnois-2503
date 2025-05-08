using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Commands;

public record DemarrerRequest((string nom, int nbGalinettes) TerrainDeChasse, List<(string nom, int nbBalles)> Chasseurs);

public class DemarrerUseCase(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Result<Guid, Error> Handle(DemarrerRequest request)
        => TerrainDeChasseValide(request.TerrainDeChasse)
            .Bind(terrain => DesChasseursValides(request.Chasseurs)
                .Map(chasseurs => (terrain, chasseurs)))
            .Ensure(result => result.chasseurs.Any(), Error.ImpossibleDeDémarrerUnePartieSansChasseurError())
            .Bind(result => Démarrer(result.terrain, result.chasseurs));

    private Result<Terrain, Error> TerrainDeChasseValide((string nom, int nbGalinettes) terrainDeChasse)
        => terrainDeChasse.nbGalinettes <= 0
            ? Error.ImpossibleDeDémarrerUnePartieSansGalinettesError()
            : new Terrain(terrainDeChasse.nom, terrainDeChasse.nbGalinettes);

    private static Result<List<Chasseur>, Error> DesChasseursValides(List<(string nom, int nbBalles)> chasseurs)
        => chasseurs.Any(chasseur => chasseur.nbBalles == 0)
            ? Error.ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalleError()
            : chasseurs
                .Select(chasseur => new Chasseur(chasseur.nom, chasseur.nbBalles))
                .ToList();

    private Result<Guid, Error> Démarrer(Terrain terrain, List<Chasseur> chasseurs)
    {
        var partieDeChasse = new PartieDeChasse(
            Guid.NewGuid(),
            PartieStatus.EnCours,
            chasseurs,
            terrain);

        partieDeChasse.Emet(new PartieDechasseDemarreEvent(timeProvider(), partieDeChasse));
        repository.Save(partieDeChasse);

        return Result.Success<Guid, Error>(partieDeChasse.Id);
    }
}
