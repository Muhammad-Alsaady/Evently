using System.Data;

namespace Evently.Common.Application.Data;

public interface IDbConnectionFactory
{
	Task<IDbConnection> OpenConnectionAsync();
}
