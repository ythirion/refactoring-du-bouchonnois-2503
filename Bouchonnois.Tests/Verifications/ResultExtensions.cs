using Bouchonnois.UseCases.Commands;

using CSharpFunctionalExtensions;

namespace Bouchonnois.Tests.Verifications;

public static class ResultExtensions
{
    public static UnitResult<Error> Succeed(this UnitResult<Error> result)
    {
        result.Should().Succeed();
        return result;
    }

    public static UnitResult<Error> FailWith(this UnitResult<Error> result, string message)
    {
        result.Should().FailWith(new Error(message));
        return result;
    }
}
