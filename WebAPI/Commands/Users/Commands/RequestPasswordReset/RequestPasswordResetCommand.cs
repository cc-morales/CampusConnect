using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.RequestPasswordReset
{
    public class RequestPasswordResetCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
