using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;

namespace UI.Services
{
    public interface ICountryService
    {
        Task<ResponseWrapper<List<CountryResponseDTO>>> GetAll();
        Task<PaginationModel<CountryResponseDTO>> GetPaginateList(FilterDTO filter);
    }
    public class CountryService : ICountryService
    {
        private readonly IHttpService _httpService;

        public CountryService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<CountryResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<CountryResponseDTO>>>("Country/Get");
        }
        public async Task<PaginationModel<CountryResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<CountryResponseDTO>>($"Country/GetPaginateList", filter);
        }
    }
}
