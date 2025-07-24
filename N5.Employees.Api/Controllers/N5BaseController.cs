using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace N5.Employees.Api.Controllers
{
	[ApiController]
	public class N5BaseController : ControllerBase
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

		protected Serilog.ILogger Logger => Log.Logger;
	}
}