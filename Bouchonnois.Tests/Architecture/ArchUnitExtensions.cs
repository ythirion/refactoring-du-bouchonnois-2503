using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.Architecture
{
    public static class ArchUnitExtensions
    {
        private static readonly ArchUnitNET.Domain.Architecture Architecture =
            new ArchLoader()
                .LoadAssemblies(typeof(PartieDeChasseService).Assembly)
                .Build();

        public static void Check(this IArchRule rule) => rule.Check(Architecture);

        public static void Check<TRuleType>(this ArchRule<TRuleType> rule)
            where TRuleType : ICanBeAnalyzed => rule.WithoutRequiringPositiveResults().Check(Architecture);
    }
}