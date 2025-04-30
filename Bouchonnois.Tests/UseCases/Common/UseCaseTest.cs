using Bouchonnois.Service;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases;

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
    protected readonly TirerUseCase TirerUseCase;
    protected readonly PrendreLapéroUseCase PrendreLapéroUseCase;
    protected readonly ReprendreLaPartieDeChasseUseCase ReprendreLaPartieDeChasseUseCase;
    protected readonly TerminerLaPartieUseCase TerminerLaPartieUseCase;
    protected readonly ConsulterStatusUseCase ConsulterStatusUseCase;


    protected UseCaseTest()
    {
        Now = DateTime.Now;
        Repository = new PartieDeChasseRepositoryForTests();
        Service = new PartieDeChasseService(Repository, () => Now);
        TirerSurGalinetteUseCase = new TirerSurGalinetteUseCase(Repository, () => Now);
        TirerUseCase = new TirerUseCase(Repository, () => Now);
        PrendreLapéroUseCase = new PrendreLapéroUseCase(Repository, () => Now);
        ReprendreLaPartieDeChasseUseCase = new ReprendreLaPartieDeChasseUseCase(Repository, () => Now);
        TerminerLaPartieUseCase = new TerminerLaPartieUseCase(Repository, () => Now);
        ConsulterStatusUseCase = new ConsulterStatusUseCase(Repository);
    }

    protected Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        Repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }

    protected Guid UnePartieDeChasseInexistante() => Guid.NewGuid();
}
