using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N5.Employees.Application.Interfaces.ElasticSearch;

namespace N5.Employees.Infrastructure.ElasticSearch.Configurations
{
	public static class AddElasticSearchServices
	{
		public static IServiceCollection AddElasticSearchService(this IServiceCollection services, IConfiguration config)
		{
			string uri = config.GetSection("Elastic")["Uri"]!;

			var elasticSettings = new ElasticsearchClientSettings(new Uri(uri)).DefaultIndex("permissions");

			var client = new ElasticsearchClient(elasticSettings);

			services.AddSingleton(client);
			services.AddSingleton<IElasticSearchService, ElasticSearchService>();
			return services;
		}
	}
}