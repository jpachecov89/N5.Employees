using Microsoft.AspNetCore.Mvc;
using N5.Employees.Application.Exceptions;
using N5.Employees.Application.Permissions.Commands.CreatePermission;
using N5.Employees.Application.Permissions.Commands.UpdatePermission;
using N5.Employees.Application.Permissions.Queries.GetPermissions;
using Serilog;

namespace N5.Employees.Api.Controllers
{
	[ApiController]
	public class PermissionController : N5BaseController
	{
		[HttpGet("api/permissions")]
		public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsQuery query)
		{
			Logger.Information($"Excuting operation: {nameof(GetPermissions)} with Payload: {query}");
			var result = await Mediator.Send(query);
			return Ok(result);
		}

		[HttpPost("api/permissions")]
		public async Task<IActionResult> RequestPermission([FromBody] CreatePermissionCommand command)
		{
			Logger.Information($"Excuting operation: {nameof(RequestPermission)} with Payload: {command}");
			var result = await Mediator.Send(command);
			return CreatedAtAction(nameof(RequestPermission), new { id = result.PermissionId }, result);
		}

		[HttpPut("api/permissions/{key}")]
		public async Task<IActionResult> ModifyPermission([FromBody] UpdatePermissionCommand command, string key)
		{
			if (!Guid.TryParse(key, out var id))
				throw new BadRequestException($"Key \"{key}\" is not a correct Id");

			Logger.Information($"Excuting operation: {nameof(ModifyPermission)} with Payload: {command} and Key: {key}");
			var fixedCommand = command with { PermissionId = id };
			var result = await Mediator.Send(fixedCommand);
			return Ok(result);
		}
	}
}