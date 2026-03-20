using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Feeds.Commands
{
    public record CreatePostCommand(NewsFeedModel NewsFeed) : IRequest<Result>;

    public class CreatePostCommandHandler : AppDatabaseBase, IRequestHandler<CreatePostCommand, Result>
    {
        public CreatePostCommandHandler(AppDbContext context) : base(context)
        {

        }

        public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {

            

            if(request.NewsFeed.Images is not null)
            {
                foreach (var image in request.NewsFeed.Images)
                {
                    GetDBContext().Entry(image).State = EntityState.Added;
                }
            }

            GetDBContext().NewsFeeds.Add(request.NewsFeed);

            await GetDBContext().SaveChangesAsync();

            return Result.Success();
        }
    }
}
