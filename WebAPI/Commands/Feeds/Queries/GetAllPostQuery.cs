using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;
using PaginationRequestModel = CamCon.Domain.PaginationRequestModel;
using PaginationResponseModel = CamCon.Domain.PaginationResponseModel;

namespace WebAPI.Commands.Feeds.Queries
{
    public record GetAllPostQuery(PaginationRequestModel Request) : IRequest<Result<PaginationResponseModel>>;

    public class GetAllPostQueryHandler(AppDbContext context) : AppDatabaseBase(context), IRequestHandler<GetAllPostQuery, Result<PaginationResponseModel>>
    {
        public async Task<Result<PaginationResponseModel>> Handle(GetAllPostQuery request, CancellationToken cancellationToken)
        {
            if (request is null) return Result.Success(new PaginationResponseModel());

            var query = GetDBContext().NewsFeeds
                .AsNoTracking()
                .AsQueryable();
            
            if (request.Request.IsGuest)
                query = query.Where(c => c.IsPublic);

            if (request.Request.MyOrganizationId is not null)
                query = query.Where(c => c.MyOrganizationId == request.Request.MyOrganizationId);
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var records = await query
                .Include(c => c.Images)
                .Include(c => c.Likes)
                .Include(c => c.MyOrganization).ThenInclude(c => c!.Contributors)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(request.Request.StartIndex)
                .Take(request.Request.Count)
                .AsSplitQuery()        
                .ToListAsync(cancellationToken);

            return Result.Success(new PaginationResponseModel
            {
                Count = totalCount,
                Records = records
            });
        }
    }
}
