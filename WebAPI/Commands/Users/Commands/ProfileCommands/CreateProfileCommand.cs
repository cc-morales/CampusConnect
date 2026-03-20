using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Users.Commands.ProfileCommands
{
    public record CreateProfileCommand(ProfileInfo profile) : IRequest<Result<ProfileInfo>>;

    public class CreateProfileCommandHandler : AppDatabaseBase, IRequestHandler<CreateProfileCommand, Result<ProfileInfo>>
    {
        public CreateProfileCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<ProfileInfo>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            await GetDBContext().ProfileInformations.AddAsync(request.profile, cancellationToken);

            await GetDBContext().SaveChangesAsync(cancellationToken);

            var profile = await GetDBContext().ProfileInformations.Include(c => c.MyOrganization).FirstOrDefaultAsync(c => c.ProfileInformationId == request.profile.ProfileInformationId);

            if(profile == null)
                return Result.Failure<ProfileInfo>(new Error(StatusCodes.Status500InternalServerError, "Error creating profile"));

            return Result.Success(profile);
        }
    }
}
