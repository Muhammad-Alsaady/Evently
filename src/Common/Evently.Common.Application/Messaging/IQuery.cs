using Evently.Common.Domain.ResultPattern;
using MediatR;

namespace Evently.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
