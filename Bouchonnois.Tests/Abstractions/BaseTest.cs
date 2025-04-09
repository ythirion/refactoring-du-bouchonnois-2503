using Bouchonnois.Domain;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Abstractions;

public abstract class BaseTest
{
    protected readonly PartieDeChasseRepositoryForTests Repository = new();
    protected Chasseur Bernard = new("Bernard", 8);
    protected Chasseur Dédé = new("Dédé", 20);
    protected Chasseur Robert = new("Robert", 12);
}
