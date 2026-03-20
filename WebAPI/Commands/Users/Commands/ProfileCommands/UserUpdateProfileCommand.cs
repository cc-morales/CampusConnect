using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Users.Commands.ProfileCommands
{
    public record UserUpdateProfileCommand(ProfileInfo profile, bool? IsAdmin = null) : IRequest<Result<ProfileInfo>>;

    public class UserUpdateProfileCommandHandler :AppDatabaseBase, IRequestHandler<UserUpdateProfileCommand, Result<ProfileInfo>>
    {
        public UserUpdateProfileCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<ProfileInfo>> Handle(UserUpdateProfileCommand request, CancellationToken cancellationToken)
        {

            var existingProfile = await GetDBContext().ProfileInformations.Include( o => o.MyOrganization)
            .FirstOrDefaultAsync(p => p.ProfileInformationId == request.profile.ProfileInformationId, cancellationToken);

            if (existingProfile == null)
                return Result.Failure<ProfileInfo>(new Error(StatusCodes.Status404NotFound, "Profile Not found."));

            if (request.IsAdmin is null)
            {
                existingProfile.ProfilePicture = request.profile.ProfilePicture;
                existingProfile.FullName = request.profile.FullName;
                existingProfile.Address = request.profile.Address;
                existingProfile.Course = request.profile.Course;
            }

            if(request.IsAdmin is not null)
                existingProfile.IsAdmin = request.IsAdmin.Value;

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success(existingProfile);
        }
    }
}
