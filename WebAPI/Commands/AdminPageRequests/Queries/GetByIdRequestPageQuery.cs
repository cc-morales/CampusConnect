using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.AdminPageRequests.Queries
{
    public record GetByIdRequestPageQuery(Guid Id) : IRequest<Result<AdminPageRequestModel>>;
    
    public class GetByIdRequestPageQueryHandler : AppDatabaseBase, IRequestHandler<GetByIdRequestPageQuery, Result<AdminPageRequestModel>>
    {
        public GetByIdRequestPageQueryHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<AdminPageRequestModel>> Handle(GetByIdRequestPageQuery request, CancellationToken cancellationToken)
        {
            var pageRequest = await GetDBContext().RequestPages.Include( c => c.PageRequestImages).FirstOrDefaultAsync(r => r.AdminPageRequestId == request.Id, cancellationToken);

            if (pageRequest == null)
            {
                return Result.Failure<AdminPageRequestModel>(new Error(404, "Admin Page Request not found"));
            }
            return Result.Success(pageRequest);
        }
    }
}
