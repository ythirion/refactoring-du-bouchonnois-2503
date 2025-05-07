namespace Bouchonnois.UseCases.Common;

public interface IUseCase<in TCommand, out TResult> where TCommand : IRequest<TResult>
{
    TResult Handle(TCommand command);
}