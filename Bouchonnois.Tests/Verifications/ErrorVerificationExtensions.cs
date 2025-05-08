using Bouchonnois.Domain;

using CSharpFunctionalExtensions;

using FluentAssertions.Primitives;

namespace Bouchonnois.Tests.Verifications;

public static class ErrorVerificationExtensions
{
    public static AndConstraint<StringAssertions> ExpectMessageToBe<TAssertion, E>(
        this AndWhichConstraint<TAssertion, E> testCase, string message)
        where TAssertion : class
        where E : Error
        => testCase.Which.Message.Should().Be(message);
}
