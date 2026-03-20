using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Comments.Commands
{
    public record AddCommentCommand(NewsFeedCommentModel Request) : IRequest<Result<NewsFeedCommentModel>>;
   
    public class AddCommentCommandHandler : AppDatabaseBase, IRequestHandler<AddCommentCommand, Result<NewsFeedCommentModel>>
    {
        public AddCommentCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<NewsFeedCommentModel>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {

            if(request.Request.User is not null)
                GetDBContext().Entry(request.Request.User).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

            await GetDBContext().NewsFeedComments.AddAsync(request.Request, cancellationToken);

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result<NewsFeedCommentModel>.Success(request.Request);
        }
    }
}
