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

    protected UseCaseTest()
    {
        Now = DateTime.Now;
        Repository = new PartieDeChasseRepositoryForTests();
    }

    protected Guid UnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();

        Repository.Add(partieDeChasse);

        return partieDeChasse.Id;
    }

    protected Guid UnePartieDeChasseInexistante() => Guid.NewGuid();
}