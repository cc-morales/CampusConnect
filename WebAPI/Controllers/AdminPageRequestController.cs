using CamCon.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Commands.AdminPageRequests.Commands;
using WebAPI.Commands.AdminPageRequests.Queries;
using WebAPI.Commands.Notifications.Queries;

namespace WebAPI.Controllers
{
    [Route("api/pagerequest/")]
    [ApiController]
    [Authorize]
    public class AdminPageRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminPageRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestAdminCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("{notificationId}")]
        public async Task<IActionResult> UpdateRequest(Guid notificationId, [FromBody] AdminPageRequestModel model)
        {
            var result = await _mediator.Send(new UpdateRequestAdminCommand(model, notificationId));

            return Ok(result);
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetById(Guid requestId)
        {
            var result = await _mediator.Send(new GetByIdRequestPageQuery(requestId));

            return Ok(result.Value);
        }
    }
}
