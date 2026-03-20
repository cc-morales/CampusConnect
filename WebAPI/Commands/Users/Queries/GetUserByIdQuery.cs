using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Users.Queries
{
    public record GetUserByIdQuery(string id) : IRequest<Result<ApplicationUserModel>>;
    
    public class GetUserByIdQueryHandler : AppDatabaseBase, IRequestHandler<GetUserByIdQuery, Result<ApplicationUserModel>>
    {
        public GetUserByIdQueryHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<ApplicationUserModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await GetDBContext().Users.Include( u => u.ProfileInformation).ThenInclude( c => c!.MyOrganization).FirstOrDefaultAsync( u => u.Id == request.id);
            if (user == null)
            {
                return Result.Failure<ApplicationUserModel>(new Error(404, "User not found"));
            }
            return Result.Success(user);
        }
    }
}
