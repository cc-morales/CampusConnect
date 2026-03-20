using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Likes.Commands
{
    public record LikeUpdateCommand(LikeModel Request) : IRequest<Result<LikeModel>>;

    public class UpdateCommandHandler : AppDatabaseBase, IRequestHandler<LikeUpdateCommand, Result<LikeModel>>
    {
        public UpdateCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<LikeModel>> Handle(LikeUpdateCommand request, CancellationToken cancellationToken)
        {
            var like = await GetDBContext().Likes.FirstOrDefaultAsync(l => l.UserId == request.Request.UserId && l.NewsFeedId == request.Request.NewsFeedId, cancellationToken);

            if(like is null)
            {
                GetDBContext().Likes.Add(request.Request);

                await GetDBContext().SaveChangesAsync(cancellationToken);

                return Result.Success(request.Request);
            } 
            else
            {

                if (like.LikeType == request.Request.LikeType)
                {
                    GetDBContext().Likes.Remove(like);

                    await GetDBContext().SaveChangesAsync(cancellationToken);

                    return Result.Success(new LikeModel()
                    {
                        Emoji = request.Request.Emoji
                    });

                }
                else
                {
                    like.LikeType = request.Request.LikeType;

                    like.Emoji = request.Request.Emoji;

                    GetDBContext().Likes.Update(like);

                    await GetDBContext().SaveChangesAsync(cancellationToken);

                    return Result.Success(like);
                }
            }
            
        }
    }
}
