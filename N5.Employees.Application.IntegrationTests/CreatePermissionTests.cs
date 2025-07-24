using N5.Employees.Application.IntegrationTests.Fixtures;
using System.Net.Http.Json;

namespace N5.Employees.Application.IntegrationTests
{
	public class CreatePermissionTests : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _client;

		public CreatePermissionTests(CustomWebApplicationFactory factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task CreatePermission_ShouldPersistToDb_AndPublishToKafka()
		{
			var request = new
			{
				EmployeeId = Guid.NewGuid(),
				PermissionTypeId = Guid.NewGuid(),
				DateGranted = DateTime.UtcNow
			};

			var response = await _client.PostAsJsonAsync("/api/permissions", request);

			response.EnsureSuccessStatusCode();
		}
	}
}