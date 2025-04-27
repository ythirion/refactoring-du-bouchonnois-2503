using Bouchonnois.Service;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases.Common;

public class UseCaseTest
{
    protected const string Dédé = "Dédé";
    protected const string Bernard = "Bernard";
    protected const string Robert = "Robert";
    protected const string ChasseurInconnu = "Chasseur inconnu";

    protected readonly DateTime Now;
    protected readonly PartieDeChasseRepositoryForTests Repository;
    protected readonly PartieDeChasseService Service;
    protected readonly TirerSurGalinetteUseCase TirerSurGalinetteUseCase;
    protected readonly DemarrerUnePartieDeChasseUseCase DemarrerUnePartieDeChasseUseCase;
    protected readonly TirerUseCase TirerUseCase;
    protected readonly PrendreLapéroUseCase PrendreLapéroUseCase;
    protected readonly ReprendreLaPartieUseCase ReprendreLaPartieUseCase;

    protected UseCaseTest()
    {
        Now = DateTime.Now;
        Repository = new PartieDeChasseRepositoryForTests();
        Service = new PartieDeChasseService(Repository, () => Now);
        TirerSurGalinetteUseCase = new TirerSurGalinetteUseCase(Repository, () => Now);
        DemarrerUnePartieDeChasseUseCase = new DemarrerUnePartieDeChasseUseCase(Repository, () => Now);
        TirerUseCase = new TirerUseCase(Repository, () => Now);
        PrendreLapéroUseCase = new PrendreLapéroUseCase(Repository, () => Now);
        ReprendreLaPartieUseCase = new ReprendreLaPartieUseCase(Repository, () => Now);
    }

    protected Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        Repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }

    protected Guid UnePartieDeChasseInexistante() => Guid.NewGuid();
}
