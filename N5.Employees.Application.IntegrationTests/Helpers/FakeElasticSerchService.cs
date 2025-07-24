using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Application.Permissions.Queries.GetPermissions;
using System.Text.Json;

namespace N5.Employees.Application.IntegrationTests.Helpers
{
	public class FakeElasticSerchService : IElasticSearchService
	{
		public Task IndexAsync(JsonElement json)
		{
			return Task.CompletedTask;
		}

		public Task IndexGetAsync(JsonElement json)
		{
			return Task.CompletedTask;
		}

		public Task<List<GetPermissionsDto>> SearchAll(GetPermissionsQuery query)
		{
			return (Task<List<GetPermissionsDto>>)Task.CompletedTask;
		}
	}
}