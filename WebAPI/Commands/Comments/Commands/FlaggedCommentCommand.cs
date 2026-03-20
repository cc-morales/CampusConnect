using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Comments.Commands
{
    public record FlaggedCommentCommand(Guid Guid) : IRequest<Result<NewsFeedCommentModel>>;

    public class FlaggedCommentCommandHandler : AppDatabaseBase, IRequestHandler<FlaggedCommentCommand, Result<NewsFeedCommentModel>>
    {
        public FlaggedCommentCommandHandler(AppDbContext context) : base(context)
        {

        }

        public async Task<Result<NewsFeedCommentModel>> Handle(FlaggedCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await GetDBContext().NewsFeedComments.FindAsync(request.Guid);

            comment.IsFlagged = true;

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success(comment);
        }


    }
}
