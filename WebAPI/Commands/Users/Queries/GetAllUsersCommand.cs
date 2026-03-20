using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Users.Commands.LoginCommand;
using WebAPI.Interfaces;
using WebAPI.Services.TokenServices;

namespace WebAPI.Commands.Users.Queries
{
    public record class GetAllUsersCommand() : IRequest<Result<List<ApplicationUserModel>>>;

    public class GetAllUsersCommandHandler : AppDatabaseBase, IRequestHandler<GetAllUsersCommand, Result<List<ApplicationUserModel>>>
    {
        public GetAllUsersCommandHandler(AppDbContext context) : base(context) { }
        public async Task<Result<List<ApplicationUserModel>>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            var users = await GetDBContext().Users
                .Include(u => u.ProfileInformation)
                .ToListAsync(cancellationToken: cancellationToken);

            return Result.Success(users);
        }
    }
}
