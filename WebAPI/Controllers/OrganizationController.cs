using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Commands.Organizations.Commands;
using WebAPI.Commands.Organizations.Query;
using WebAPI.Commands.Users.Commands.CreateCommand;

namespace WebAPI.Controllers
{
    [Route("api/org/")]
    [ApiController]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly ISender _mediator;

        public OrganizationController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetOrganizations()
        {
            var result = await _mediator.Send(new GetOrganizationsQuery());

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> Updateorganization([FromBody] UpdateOrganizationCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("contributor")]
        public async Task<IActionResult> AddContributor([FromBody] AddContributorCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPatch("{organizationId}/follow")]
        public async Task<IActionResult> ToggleFollow(Guid organizationId, [FromBody] ToggleFollowRequest request)
        {
            var result = await _mediator.Send(new ToggleFollowCommand(organizationId, request.UserId));

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.Error.Code, result.Error.Description);
        }
    }

    public record ToggleFollowRequest(string UserId);
}
