using FsCheck;

using JetBrains.Annotations;

using GroupDeChasseurs = (string nom, int nbBalles)[];

namespace Bouchonnois.Tests.UseCases;

public static class DesChasseursAvecDesBalles
{
    [UsedImplicitly]
    public static Arbitrary<GroupDeChasseurs> Generate() => ArbitraryExtensions.DesChasseursAvecDesBalles();
}
