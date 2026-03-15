using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Admin.Queries
{
    public record GetAllAdminsQuery() : IRequest<Result<List<AdminAccountDto>>>;

    /// <summary>
    /// DTO returned to the client for admin accounts.
    /// </summary>
    public class AdminAccountDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string[] AccessControl { get; set; } = [];
    }

    public class GetAllAdminsQueryHandler : AppDatabaseBase, IRequestHandler<GetAllAdminsQuery, Result<List<AdminAccountDto>>>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;

        public GetAllAdminsQueryHandler(AppDbContext context, UserManager<ApplicationUserModel> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<Result<List<AdminAccountDto>>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(Roles.Admin);

            var result = adminUsers.Select(u => new AdminAccountDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? string.Empty,
                UserName = u.UserName ?? string.Empty,
                AccessControl = string.IsNullOrEmpty(u.AccessControl)
                    ? []
                    : u.AccessControl.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            }).ToList();

            return Result.Success(result);
        }
    }
}

