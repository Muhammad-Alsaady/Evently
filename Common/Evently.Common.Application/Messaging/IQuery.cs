using Evently.Common.Domain.Results;
using Waseet.CQRS;

namespace Evently.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery;

public interface IBaseQuery;
