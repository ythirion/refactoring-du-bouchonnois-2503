using CSharpFunctionalExtensions;

namespace Bouchonnois.Domain;

public static class UnitResultExtensions
{
    public static UnitResult<Error> Success() => UnitResult.Success<Error>();

}
