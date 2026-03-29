using CamCon.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Commands.Feeds.Commands;
using WebAPI.Commands.Feeds.Queries;

namespace WebAPI.Controllers
{
    public record UpdatePostVisibilityRequest(bool IsPublic);

    [Route("api/post/")]
    [ApiController]
    [Authorize]
    public class NewsFeedController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsFeedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromBody] CreatePostCommand command)
        {
            var result = await _mediator.Send(command);

            if(result.IsSuccess)
                return Ok(result);

            return StatusCode(500);
        }

        [HttpPost("getall")]
        public async Task<IActionResult> GetPosts([FromBody] GetAllPostQuery request)
        {
            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(500);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(500);
        }

        [HttpDelete("{newsFeedId}")]
        public async Task<IActionResult> DeletePost(Guid newsFeedId)
        {
            var result = await _mediator.Send(new DeletePostCommand(newsFeedId));

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(500);
        }

        [HttpPatch("{newsFeedId}/visibility")]
        public async Task<IActionResult> UpdatePostVisibility(Guid newsFeedId, [FromBody] UpdatePostVisibilityRequest request)
        {
            var result = await _mediator.Send(new UpdatePostVisibilityCommand(newsFeedId, request.IsPublic));

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.Error.Code, result.Error.Description);
        }

        //[HttpGet("/{organizationId}")]
        //[Authorize]
        //public async Task<IActionResult> GetPostByOrganizationId(string organizationId)
        //{
        //    var result = await _mediator.Send(command);

        //    if (result.IsSuccess)
        //        return Ok(result);

        //    return StatusCode(500);
        //}
    }
}
