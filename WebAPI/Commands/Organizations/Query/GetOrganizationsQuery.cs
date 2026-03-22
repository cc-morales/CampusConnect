using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Notifications.Events;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Query
{
    public record GetOrganizationsQuery : IRequest<Result<List<MyOrganizationModel>>>;
    
    public class GetOrganizationsQueryHandler : AppDatabaseBase, IRequestHandler<GetOrganizationsQuery, Result<List<MyOrganizationModel>>>
    {
        private readonly IMediator mediator;
        public GetOrganizationsQueryHandler(AppDbContext context, IMediator mediator) : base(context) => this.mediator = mediator;

        public async Task<Result<List<MyOrganizationModel>>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            var organizations = await GetDBContext().MyOrganizations
                .AsNoTracking()
                .AsSplitQuery()
                .Select(o => new MyOrganizationModel
                {
                    MyOrganizationId = o.MyOrganizationId,
                    OrganizationName = o.OrganizationName,
                    Description = o.Description,
                    OrganizationType = o.OrganizationType,
                    Id = o.Id,
                    User = o.User == null ? null : new ApplicationUserModel
                    {
                        Id = o.User.Id,
                        UserName = o.User.UserName,
                        Name = o.User.Name,
                        ProfileInformationId = o.User.ProfileInformationId,
                        IsWebCreated = o.User.IsWebCreated,
                        ProfileInformation = o.User.ProfileInformation == null ? null : new ProfileInfo
                        {
                            ProfileInformationId = o.User.ProfileInformation.ProfileInformationId,
                            IsAdmin = o.User.ProfileInformation.IsAdmin,
                            ProfilePicture = o.User.ProfileInformation.ProfilePicture,
                            CoverPicture = o.User.ProfileInformation.CoverPicture,
                            FullName = o.User.ProfileInformation.FullName,
                            Address = o.User.ProfileInformation.Address,
                            Course = o.User.ProfileInformation.Course,
                            MyOrganization = null
                        }
                    },
                    OrganizationDepartmentId = o.OrganizationDepartmentId,
                    OrganizationDepartment = o.OrganizationDepartment == null ? null : new OrganizationDepartmentModel
                    {
                        OrganizationDepartmentId = o.OrganizationDepartment.OrganizationDepartmentId,
                        MyOrganizationId = o.OrganizationDepartment.MyOrganizationId
                    },
                    Photo = o.Photo,
                    CoverPhoto = o.CoverPhoto,
                    Contributors = o.Contributors.Select(c => new Contributors
                    {
                        ContributorsId = c.ContributorsId,
                        MyOrganizationId = c.MyOrganizationId,
                        Id = c.Id
                    }).ToList(),
                    Followers = o.Followers.Select(f => new Follower
                    {
                        FollowerId = f.FollowerId,
                        MyOrganizationId = f.MyOrganizationId,
                        Id = f.Id
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return Result.Success(organizations);
        }
    }
}
