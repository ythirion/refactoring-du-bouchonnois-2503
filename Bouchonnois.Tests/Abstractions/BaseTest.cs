using Bouchonnois.Domain;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Abstractions;

public abstract class BaseTest
{
    protected readonly PartieDeChasseRepositoryForTests Repository = new();
    public Chasseur Bernard = new("Bernard", 8);
    public Chasseur Dédé = new("Dédé", 20);
}
