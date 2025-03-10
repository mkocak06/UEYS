using Application.Services;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }
        
        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, [FromBody] FilterDTO filter) 
            => Ok(await _countryService.GetPaginateList(cancellationToken, filter));

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetListPagination)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        //    => Ok(await _countryService.GetListAsync(cancellationToken));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.CountryGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) 
        //    => Ok(await _countryService.GetByIdAsync(cancellationToken, id));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.CountryAdd)]
        //public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] CountryDTO countryDto) 
        //    => Ok(await _countryService.PostAsync(cancellationToken, countryDto));

        //[HttpPut("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.CountryUpdate)]
        //public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] CountryDTO countryDto) 
        //    => StatusCode(201, await _countryService.Put(cancellationToken, id, countryDto));

        //[HttpDelete("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.CountryDelete)]
        //public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) 
        //    => Ok(await _countryService.Delete(cancellationToken, id));
    }
}
