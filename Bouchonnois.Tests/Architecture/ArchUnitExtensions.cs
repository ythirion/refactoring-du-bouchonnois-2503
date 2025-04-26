using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;

using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Architecture;

public static class ArchUnitExtensions
{
    private readonly static ArchUnitNET.Domain.Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(typeof(PartieDeChasse).Assembly)
            .Build();

    public static void Check(this IArchRule rule) => rule.Check(Architecture);

    public static void Check<TRuleType>(this ArchRule<TRuleType> rule)
        where TRuleType : ICanBeAnalyzed => rule.WithoutRequiringPositiveResults().Check(Architecture);
}
