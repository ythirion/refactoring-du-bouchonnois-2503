using Bouchonnois.Service;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.UseCases;

public class UseCaseTest
{
    protected const string Dédé = "Dédé";
    protected const string Bernard = "Bernard";
    protected const string Robert = "Robert";
    protected const string ChasseurInconnu = "Chasseur inconnu";

    protected readonly DateTime Now = DateTime.Now;
    protected readonly PartieDeChasseRepositoryForTests Repository = new();
    protected readonly PartieDeChasseService Service;

    protected UseCaseTest() => Service = new PartieDeChasseService(Repository, () => Now);
    protected Guid UnePartieDeChasseInexistante() => Guid.NewGuid();

    protected Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        Repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }
}