using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.LoginCommand
{
    public record class UserLoginCommand(LoginModel? request) : IRequest<Result<TokenModel>>;
}
