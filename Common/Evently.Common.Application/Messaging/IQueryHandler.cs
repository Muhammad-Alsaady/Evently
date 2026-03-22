using Evently.Common.Domain.Results;
using Waseet.CQRS;

namespace Evently.Common.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
