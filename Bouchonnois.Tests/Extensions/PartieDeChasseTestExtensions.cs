using Bouchonnois.Domain;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;

namespace Bouchonnois.Tests.Extensions;

internal static class PartieDeChasseTestExtensions
{
    internal static AndConstraint<StringAssertions> AssertEventOccuredWithMessage(this PartieDeChasse partieDeChasse, string message)
        => partieDeChasse.Events.Should()
            .ContainSingle()
            .Which
            .Message.Should().Be(message);

    internal static AndConstraint<GenericCollectionAssertions<Event>> AssertEventsAreEquivalentTo(this PartieDeChasse partieDeChasse, Event[] @events)
        => partieDeChasse.Events
            .Should()
            .BeEquivalentTo(@events);
    
    internal static  AndConstraint<GenericCollectionAssertions<Event>> AssertNoEventOccured(this PartieDeChasse partieDeChasse)
        => partieDeChasse.Events.Should().BeEmpty();
}