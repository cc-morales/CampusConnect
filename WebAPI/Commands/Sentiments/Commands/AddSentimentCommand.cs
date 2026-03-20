using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Sentiments.Commands
{
    public record AddSentimentCommand(SentimentModel Sentiment) : IRequest<Result>;

    public class AddSentimentCommandHandler : AppDatabaseBase, IRequestHandler<AddSentimentCommand, Result>
    {
        public AddSentimentCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result> Handle(AddSentimentCommand request, CancellationToken cancellationToken)
        {
            await GetDBContext().Sentiments.AddAsync(request.Sentiment);

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
