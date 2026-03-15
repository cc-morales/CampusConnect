using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Commands.Admin.Commands;
using WebAPI.Commands.Admin.Queries;

namespace WebAPI.Controllers
{
    [Route("api/admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var result = await _mediator.Send(new GetAllAdminsQuery());

            if (result.IsFailure)
                return StatusCode(result.Error.Code, result.Error.Description);

            return Ok(result.Value);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.Error.Code, result.Error.Description);
        }

        [HttpPut("access")]
        public async Task<IActionResult> UpdateAccess([FromBody] UpdateAdminAccessCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.Error.Code, result.Error.Description);
        }
    }
}

