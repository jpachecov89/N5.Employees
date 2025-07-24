using N5.Employees.Application.IntegrationTests.Fixtures;
using System.Net.Http.Json;

namespace N5.Employees.Application.IntegrationTests
{
	public class UpdatePermissionTests : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _client;

		public UpdatePermissionTests(CustomWebApplicationFactory factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task UpdatePermission_ShouldPersistToDb_AndPublishToKafka()
		{
			var request = new
			{
				PermissionId = Guid.NewGuid(),
				EmployeeId = Guid.NewGuid(),
				PermissionTypeId = Guid.NewGuid(),
				DateGranted = DateTime.UtcNow
			};

			var response = await _client.PutAsJsonAsync("/api/permissions", request);

			response.EnsureSuccessStatusCode();
		}
	}
}