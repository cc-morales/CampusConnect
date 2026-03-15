using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result>
    {
        public string Guid { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
