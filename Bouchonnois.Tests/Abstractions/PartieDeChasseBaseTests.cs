using Bouchonnois.Tests.Doubles;
using Bouchonnois.Tests.UseCases.DataBuilders;

namespace Bouchonnois.Tests.Abstractions;

public abstract class PartieDeChasseBaseTests
{
    protected readonly ChasseurBuilder Bernard = new ChasseurBuilder(Data.Bernard)
        .AvecDesBalles(8);

    protected readonly ChasseurBuilder Dédé = new ChasseurBuilder(Data.Dédé)
        .AvecDesBalles(20);

    protected readonly PartieDeChasseRepositoryForTests Repository = new();

    protected readonly ChasseurBuilder Robert = new ChasseurBuilder(Data.Robert)
        .AvecDesBalles(12);

    protected static PartieDeChasseBuilder UnePartieDeChasseEnCours => PartieDeChasseBuilder.UnePartieDeChasseEnCours;

    protected static PartieDeChasseBuilder UnePartieDeChasseAlApero => PartieDeChasseBuilder.UnePartieDeChasseALapero;

    protected static PartieDeChasseBuilder UnePartieDeChasseTerminée => PartieDeChasseBuilder.UnePartieDeChasseTerminée;
}
