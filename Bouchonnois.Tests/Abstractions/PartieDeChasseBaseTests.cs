using Bouchonnois.Domain;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.UseCases;

namespace Bouchonnois.Tests.Abstractions;

public abstract class PartieDeChasseBaseTests
{
    protected PartieDeChasseRepositoryForTests Repository = new();

    protected Chasseur Dédé = new(Data.Dédé, 20);
    protected Chasseur Bernard = new(Data.Bernard, 8);
    protected Chasseur Robert = new(Data.Robert, 12);

    protected static PartieDeChasseBuilder UnePartieDeChasseEnCours => PartieDeChasseBuilder.UnePartieDeChasseEnCours;

    protected static PartieDeChasseBuilder UnePartieDeChasseALApero => PartieDeChasseBuilder.UnePartieDeChasseALapero;

    protected static PartieDeChasseBuilder UnePartieDeChasseTerminée => PartieDeChasseBuilder.UnePartieDeChasseTerminée;
}
