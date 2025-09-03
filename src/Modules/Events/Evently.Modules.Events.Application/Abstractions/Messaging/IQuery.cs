using Evently.Common.Domain.ResultPattern;
using MediatR;

namespace Evently.Modules.Events.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
