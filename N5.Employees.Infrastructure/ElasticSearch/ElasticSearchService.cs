using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Application.Permissions.Queries.GetPermissions;
using System.Text.Json;

namespace N5.Employees.Infrastructure.ElasticSearch
{
	public class ElasticSearchService : IElasticSearchService
	{
		private readonly ILogger<ElasticSearchService> _logger;
		private readonly ElasticsearchClient _client;

		public ElasticSearchService(ElasticsearchClient client, ILogger<ElasticSearchService> logger)
		{
			_logger = logger;
			_client = client;
		}

		public async Task IndexAsync(JsonElement json)
		{
			string id = json.GetProperty("Id").GetString()!;
			_logger.LogInformation("Indexing document {0} into index {1}", id, "permissions");
			await _client.IndexAsync(json, i => i.Index("permissions").Id(id));
		}

		public async Task IndexGetAsync(JsonElement json)
		{
			string id = json.GetProperty("Id").GetString()!;
			_logger.LogInformation("Indexing document {0} into index {1}", id, "get-permissions");
			await _client.IndexAsync(json, i => i.Index("get-permissions").Id(id));
		}

		public async Task<List<GetPermissionsDto>> SearchAll(GetPermissionsQuery query)
		{
			var mustQueries = new List<Query>();

			if (query.EmployeeId.HasValue)
			{
				mustQueries.Add(new MatchQuery("EmployeeId"!)
				{
					Query = query.EmployeeId!.Value.ToString()
				});
			}

			if (!string.IsNullOrEmpty(query.EmployeeName))
			{
				mustQueries.Add(new WildcardQuery("EmployeeName"!)
				{
					Value = $"*{query.EmployeeName}*"
				});
			}

			if (query.PermissionTypeId.HasValue)
			{
				mustQueries.Add(new MatchQuery("PermissionTypeId"!)
				{
					Query = query.PermissionTypeId!.Value.ToString()
				});
			}

			if (!string.IsNullOrEmpty(query.PermissionTypeName))
			{
				mustQueries.Add(new WildcardQuery("PermissionTypeName"!)
				{
					Value = $"*{query.PermissionTypeName}*"
				});
			}

			if (query.FromDate.HasValue || query.ToDate.HasValue)
			{
				mustQueries.Add(new DateRangeQuery("DateGranted"!)
				{
					Gte = query.FromDate,
					Lte = query.ToDate
				});
			}

			var boolQuery = new BoolQuery { Must = mustQueries };

			_logger.LogInformation("Searching permissions with filters: {0}", query);

			var response = await _client.SearchAsync<JsonElement>(s => s.Index("permissions").Query(boolQuery));

			if (!response.IsValidResponse || response.Documents == null)
				return new List<GetPermissionsDto>();
			return response.Documents.Select(x => new GetPermissionsDto
			(
				x.GetProperty("Id").GetGuid(),
				x.GetProperty("EmployeeName").GetString()!,
				x.GetProperty("PermissionTypeName").GetString()!,
				x.GetProperty("DateGranted").GetDateTime()
			)).ToList();
		}
	}
}