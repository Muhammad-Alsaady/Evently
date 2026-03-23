using Evently.Common.Domain.Results;
using Waseet.CQRS;

namespace Evently.Common.Application.Messaging;

public interface ICommand : IRequest<Result<Success>>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
