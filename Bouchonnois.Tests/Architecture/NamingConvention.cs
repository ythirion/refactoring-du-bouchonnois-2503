namespace Bouchonnois.Tests.Architecture;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

public class NamingConvention
{
    [Fact]
    public void AllMethodsShouldStartWithACapitalLetter()
        => MethodMembers()
            .That()
            .AreNoConstructors()
            .And()
            .DoNotHaveAnyAttributes("SpecialName|CompilerGenerated", true)
            .Should()
            .HaveName(@"^[A-Z]", true)
            .Because("C# convention...")
            .Check();

    [Fact]
    public void InterfacesShouldStartWithI()
        => Assert.Fail("FIX ME");
}
