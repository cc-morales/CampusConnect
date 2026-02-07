using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public ChangePasswordModel? Request { get; set; }
    }
}