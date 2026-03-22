using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.GuestLogin
{
    public record class GuestLoginCommand() : IRequest<Result<TokenModel>>;
}

