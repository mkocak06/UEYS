using AspNetCoreRateLimit;
using Core.Extentsions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Middlewares;

public class CustomRateLimitConfiguration : RateLimitConfiguration
{
    public CustomRateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions,
        IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions)
    {
    }

    public override void RegisterResolvers()
    {
        //base.RegisterResolvers();
        ClientResolvers.Add(new ClientQueryStringResolveContributor());
    }
}

public class ClientQueryStringResolveContributor : IClientResolveContributor
{
    public Task<string> ResolveClientAsync(HttpContext httpContext)
    {
        try
        {
            var userId = httpContext.GetUserId();
            return Task.FromResult("cl-key-" + userId);
        }
        catch
        {
            return Task.FromResult("cl-key-0");
        }
    }
}