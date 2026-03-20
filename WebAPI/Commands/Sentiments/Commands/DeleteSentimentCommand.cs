using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Sentiments.Commands
{
    public record DeleteSentimentCommand(SentimentModel Sentiment) : IRequest<Result>;

    public class DeleteSentimentCommandHandler : AppDatabaseBase, IRequestHandler<DeleteSentimentCommand, Result>
    {
        public DeleteSentimentCommandHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result> Handle(DeleteSentimentCommand request, CancellationToken cancellationToken)
        {
            var sentiment = await GetDBContext().Sentiments.FirstOrDefaultAsync(c => c.SentimentId == request.Sentiment.SentimentId);

            if(sentiment is null)
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "Sentiment not found"));

            GetDBContext().Sentiments.Remove(sentiment);

            await GetDBContext().SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
