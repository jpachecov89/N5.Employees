using N5.Employees.Application.Permissions.Queries.GetPermissions;
using System.Text.Json;

namespace N5.Employees.Application.Interfaces.ElasticSearch
{
	public interface IElasticSearchService
	{
		Task IndexAsync(JsonElement json);
		Task IndexGetAsync(JsonElement json);
		Task<List<GetPermissionsDto>> SearchAll(GetPermissionsQuery query);
	}
}