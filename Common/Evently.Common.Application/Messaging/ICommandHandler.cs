using Evently.Common.Domain.Results;
using Waseet.CQRS;

namespace Evently.Common.Application.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result<Success>>
    where TCommand : ICommand;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
