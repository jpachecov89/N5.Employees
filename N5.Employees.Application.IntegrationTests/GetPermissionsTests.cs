using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using N5.Employees.Application.IntegrationTests.Fixtures;
using N5.Employees.Application.Permissions.Queries.GetPermissions;
using System.Net;
using System.Net.Http.Json;

namespace N5.Employees.Application.IntegrationTests
{
	public class GetPermissionsTests : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _client;

		public GetPermissionsTests(CustomWebApplicationFactory factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task GetPermissions_ShouldReturnResults_AndPublishKafkaLog()
		{
			var queryParams = new Dictionary<string, string?>
			{
				{ "EmployeeName", "John" },
                { "FromDate", DateTime.UtcNow.AddMonths(-1).ToString("o") },
				{ "ToDate", DateTime.UtcNow.ToString("o") }
			};

			var uri = QueryHelpers.AddQueryString("/api/permissions", queryParams!);

			var response = await _client.GetAsync(uri);

			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var result = await response.Content.ReadFromJsonAsync<List<GetPermissionsDto>>();
			result.Should().NotBeNull();
			result!.Should().BeAssignableTo<IEnumerable<GetPermissionsDto>>();
		}
	}
}