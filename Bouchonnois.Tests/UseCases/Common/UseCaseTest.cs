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

    protected UseCaseTest()
    {
        Now = DateTime.Now;
        Repository = new PartieDeChasseRepositoryForTests();
        Service = new PartieDeChasseService(Repository, () => Now);
    }
    
    protected Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        Repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }

    protected Guid UnePartieDeChasseInexistante() => Guid.NewGuid();
}