using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Comments.Commands
{
    public record DeleteCommentCommand(Guid Guid) : IRequest<Result<NewsFeedCommentModel>>;

    public class DeleteCommentCommandHandler : AppDatabaseBase, IRequestHandler<DeleteCommentCommand, Result<NewsFeedCommentModel>>
    {
        public DeleteCommentCommandHandler(AppDbContext context) : base(context)
        {

        }

        public async Task<Result<NewsFeedCommentModel>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await GetDBContext().NewsFeedComments.FindAsync(request.Guid);

            comment.IsDeleted = true;

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success(comment);
        }


    }
}
