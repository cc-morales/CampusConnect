using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;

namespace WebAPI.Commands.Users.Commands.CreateCommand
{
    public record class CreateAccountCommand(SignupModel? Request) : IRequest<Result>;
}
