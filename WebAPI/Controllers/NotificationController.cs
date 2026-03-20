using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Commands.Notifications.Commands;
using WebAPI.Commands.Notifications.Queries;

namespace WebAPI.Controllers
{
    [Route("api/notify/")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly ISender _mediator;

        public NotificationController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _mediator.Send(new GetAllNotificationsQuery());

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetByIdNotificationQuery(id));

            return Ok(result.Value);
        }

        [HttpGet("recipient/{id}")]
        public async Task<IActionResult> GetByRecipient(string id)
        {
            var result = await _mediator.Send(new GetByIdReciepientNotificationQuery(id));

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id, [FromQuery] bool isAdmin = false)
        {
            var result = await _mediator.Send(new DeleteNotificationCommand(id, isAdmin));

            return Ok(result);
        }

        [HttpDelete("clear/{recipientUserId}")]
        public async Task<IActionResult> ClearNotifications(string recipientUserId)
        {
            var result = await _mediator.Send(new ClearNotificationsCommand(recipientUserId));

            return Ok(result);
        }

        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllNotifications()
        {
            var result = await _mediator.Send(new ClearAllNotificationsCommand());

            return Ok(result);
        }
    }
}
