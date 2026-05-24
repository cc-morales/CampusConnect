using CamCon.Shared;
using Google.Apis.Auth.OAuth2;
using MediatR;

namespace WebAPI.Commands.Google;

public record GetGoogleTokenQuery : IRequest<Result<string>>;

public class GetGoogleTokenQueryHandler : IRequestHandler<GetGoogleTokenQuery, Result<string>>
{
    [Obsolete("Obsolete")]
    public async Task<Result<string>> Handle(GetGoogleTokenQuery request, CancellationToken cancellationToken)
    {
        var credential = GoogleCredential.FromFile("service-account.json")
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        return accessToken;
    }
}