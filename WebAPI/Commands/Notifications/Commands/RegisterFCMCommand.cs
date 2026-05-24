using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Notifications.Commands;

public record RegisterFcmCommand(DeviceFcm request) : IRequest<Result>;

public class RegisterFcmCommandHandler(AppDbContext context) : AppDatabaseBase(context), IRequestHandler<RegisterFcmCommand, Result>
{
    private readonly AppDbContext _context = context;

    public async Task<Result> Handle(RegisterFcmCommand request, CancellationToken cancellationToken)
    {
        var fcm = _context.DeviceFcms.FirstOrDefault(c => c.UserId == request.request.UserId);

        if (fcm is not null) return Result.Success();
        
        await _context.DeviceFcms.AddAsync(new DeviceFcm
        {
            UserId = request.request.UserId,
            FcmToken = request.request.FcmToken
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}