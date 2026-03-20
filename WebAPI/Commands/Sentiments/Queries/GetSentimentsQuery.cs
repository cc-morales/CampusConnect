using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Sentiments.Queries
{
    public record GetSentimentsQuery : IRequest<Result<List<SentimentModel>>>;

    public class GetSentimentsQueryHandler : AppDatabaseBase, IRequestHandler<GetSentimentsQuery, Result<List<SentimentModel>>>
    {
        public GetSentimentsQueryHandler(AppDbContext context) : base(context)
        {
        }

        public async Task<Result<List<SentimentModel>>> Handle(GetSentimentsQuery request, CancellationToken cancellationToken)
        {
            var sentiments = await GetDBContext().Sentiments.ToListAsync(cancellationToken);

            return Result.Success(sentiments);
        }
    }
}
