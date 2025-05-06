using Bouchonnois.Domain;
using Bouchonnois.Domain.Common;
using CSharpFunctionalExtensions;

namespace Bouchonnois.UseCases.Common;

public interface ICommandUseCase<in TCommand> where TCommand : ICommand
{
    UnitResult<Error> Handle(TCommand command);
}