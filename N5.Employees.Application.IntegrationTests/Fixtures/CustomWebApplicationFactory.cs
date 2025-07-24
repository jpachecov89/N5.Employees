using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N5.Employees.Api;
using N5.Employees.Application.IntegrationTests.Helpers;
using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Infrastructure.Kafka;

namespace N5.Employees.Application.IntegrationTests.Fixtures
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var hosted = services.SingleOrDefault(x =>
					x.ServiceType == typeof(IHostedService) &&
					x.ImplementationType == typeof(KafkaConsumerService));
				if (hosted != null)
					services.Remove(hosted);

				var kafka = services.SingleOrDefault(x =>
					x.ServiceType == typeof(IKafkaProducerSerivce));
				if (kafka != null)
					services.Remove(kafka);

				var elastic = services.SingleOrDefault(x =>
					x.ServiceType == typeof(IElasticSearchService));
				if (elastic != null)
					services.Remove(elastic);

				services.AddSingleton<IKafkaProducerSerivce, FakeKafkaProducer>();
				services.AddSingleton<IElasticSearchService, FakeElasticSerchService>();
			});
		}
	}
}