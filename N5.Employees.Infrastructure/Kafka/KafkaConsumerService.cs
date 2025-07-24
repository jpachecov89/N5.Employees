using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Infrastructure.Kafka.Configurations;
using System.Text.Json;

namespace N5.Employees.Infrastructure.Kafka
{
	public class KafkaConsumerService : BackgroundService
	{
		private readonly ILogger<KafkaConsumerService> _logger;
		private readonly KafkaSettings _settings;
		private readonly IElasticSearchService _elastic;

		public KafkaConsumerService(
			ILogger<KafkaConsumerService> logger,
			IOptions<KafkaSettings> settings,
			IElasticSearchService elastic
		)
		{
			_logger = logger;
			_settings = settings.Value;
			_elastic = elastic;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await Task.Yield();
			var config = new ConsumerConfig
			{
				BootstrapServers = _settings.BootstrapServers,
				GroupId = "n5-permission-consumers",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = true
			};

			using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
			consumer.Subscribe(_settings.Topic);

			_logger.LogInformation("KafkaConsumerService started. Listening topic: {0}", _settings.Topic);

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						var cr = consumer.Consume(cancellationToken);
						var messageJson = cr.Message.Value;

						var evt = JsonSerializer.Deserialize<OperationLogDto>(messageJson);
						if (evt is null)
						{
							_logger.LogWarning("Invalid message: {0}", messageJson);
							continue;
						}

						await ProcessEventAsync(evt, cancellationToken);
					}
					catch (ConsumeException ex)
					{
						_logger.LogError(ex, "Error consuming message of Kafka.");
					}
					catch (OperationCanceledException ex)
					{
						_logger.LogError("Kafka consumer canceled: {0}", ex.Message);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error processing message.");
					}
				}
			}
			finally
			{
				consumer.Close();
			}
		}

		private async Task ProcessEventAsync(OperationLogDto evt, CancellationToken cancellationToken)
		{
			switch (evt.Operation?.ToLowerInvariant())
			{
				case "request":
				case "modify":
					if (!string.IsNullOrEmpty(evt.Payload))
					{
						try
						{
							var permission = JsonSerializer.Deserialize<JsonElement>(evt.Payload);
							if (permission.ValueKind == JsonValueKind.Null || permission.ValueKind == JsonValueKind.Undefined)
							{
								_logger.LogWarning("Payload empty for this event {0}.", evt.Id);
							}
							else
							{
								string id = permission.GetProperty("Id").GetString()!;
								await _elastic.IndexAsync(permission);
								_logger.LogInformation("Indexed permission {0}", id);
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Error indexing event on {0}.", evt.Id);
						}
					}
					break;
				case "get":
					if (!string.IsNullOrEmpty(evt.Payload))
					{
						try
						{
							var query = JsonSerializer.Deserialize<JsonElement>(evt.Payload);
							if (query.ValueKind == JsonValueKind.Null || query.ValueKind == JsonValueKind.Undefined)
							{
								_logger.LogWarning("Payload GET empty for this event {0}.", evt.Id);
							}
							else
							{
								string id = query.GetProperty("Id").GetString()!;
								await _elastic.IndexGetAsync(query);
								_logger.LogInformation("Indexed GET permission {0}", id);
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Error indexing GET event {0}.", evt.Id);
						}
					}
					_logger.LogInformation("Audit GET: Permission {0}", evt.Id);
					break;

				default:
					_logger.LogWarning("Unkown Operation on event {0}: {1}", evt.Id, evt.Operation);
					break;
			}
		}
	}
}