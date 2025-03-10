using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Core.Extentsions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientRateLimitController : BaseController
    {
        private readonly ClientRateLimitOptions _options;
        private readonly IClientPolicyStore _clientPolicyStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientRateLimitController(IOptions<ClientRateLimitOptions> optionsAccessor, IClientPolicyStore clientPolicyStore, IHttpContextAccessor httpContextAccessor)
        {
            _options = optionsAccessor.Value;
            _clientPolicyStore = clientPolicyStore;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ClientRateLimitPolicy> Get()
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            return await _clientPolicyStore.GetAsync($"{_options.ClientPolicyPrefix}_cl-key-{userId}", HttpContext.RequestAborted);
        }

        [HttpPost]
        public async Task Post()
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var id = $"{_options.ClientPolicyPrefix}_cl-key-{userId}";
            var policy = await _clientPolicyStore.GetAsync(id, HttpContext.RequestAborted);

            policy.Rules.Add(new RateLimitRule
            {
                Endpoint = "*/api/Title/GetList",
                Period = "20s",
                Limit = 3
            });

            await _clientPolicyStore.SetAsync(id, policy, cancellationToken: HttpContext.RequestAborted);
        }
    }
}
