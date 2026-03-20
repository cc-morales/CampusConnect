using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Organizations.Commands
{
    public record class CreateDepartmentCommand(DepartmentModel Department) : IRequest<Result>;

    public class CreateDepartmentCommandHandler : AppDatabaseBase, IRequestHandler<CreateDepartmentCommand, Result>
    {
        public CreateDepartmentCommandHandler(AppDbContext context)  : base(context) { }
        public async Task<Result> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            GetDBContext().Departments.Add(request.Department);

            await GetDBContext().SaveChangesAsync();

            return Result.Success();
        }
    }
}
