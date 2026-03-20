using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Departments.Queries
{
    public record GetDepartmentsQuery() : IRequest<Result<List<DepartmentModel>>>;

    public class GetDepartmentsQueryHandler : AppDatabaseBase, IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentModel>>>
    {
        public GetDepartmentsQueryHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<List<DepartmentModel>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await GetDBContext().Departments.ToListAsync(cancellationToken: cancellationToken);

            return Result.Success(departments);
        }
    }
}
