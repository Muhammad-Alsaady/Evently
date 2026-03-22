using Waseet.CQRS;
using Evently.Common.Domain.Results;

namespace Evently.Common.Application.Messaging;

public interface ICommand : IRequest<Result<Success>>, IBaseCommand;

public interface ICommandHandler<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
