using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
	public interface ICountryService
    {
        Task<PaginationModel<CountryResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<CountryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<CountryResponseDTO>> PostAsync(CancellationToken cancellationToken, CountryDTO countryDto);
        Task<ResponseWrapper<CountryResponseDTO>> Put(CancellationToken cancellationToken, long id, CountryDTO countryDto);
        Task<ResponseWrapper<CountryResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<CountryResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
    }
}
