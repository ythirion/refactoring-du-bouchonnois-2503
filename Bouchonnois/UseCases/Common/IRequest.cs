using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Common;

public interface IRequest : IRequest<UnitResult<Error>>;

public interface IRequest<out TResult>;