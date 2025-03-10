using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Pages.Management.Property;

namespace UI.Services;

public interface IPropertyService
{
    Task<ResponseWrapper<List<PropertyResponseDTO>>> GetByType(PropertyType propertyType, PerfectionType? perfectionType, string queryText);
	Task<ResponseWrapper<List<PropertyResponseDTO>>> GetAll();
	Task<PaginationModel<PropertyResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<PropertyResponseDTO>> GetById(long id);
    Task<ResponseWrapper<PropertyResponseDTO>> Update(long id, PropertyDTO property);
    Task<ResponseWrapper<PropertyResponseDTO>> Add(PropertyDTO property);
    Task Delete(long id);
}

public class PropertyService : IPropertyService
{
	private readonly IHttpService _httpService;

	public PropertyService(IHttpService httpService)
	{
		_httpService = httpService;
	}
	public async Task<ResponseWrapper<List<PropertyResponseDTO>>> GetByType(PropertyType propertyType, PerfectionType? perfectionType, string queryText)
	{
		return await _httpService.Get<ResponseWrapper<List<PropertyResponseDTO>>>($"Property/GetByType?propertyType={propertyType}&perfectionType={perfectionType}&queryText={queryText}");
	}
	public async Task<PaginationModel<PropertyResponseDTO>> GetPaginateList(FilterDTO filter)
	{
		return await _httpService.Post<PaginationModel<PropertyResponseDTO>>($"Property/GetPaginateList", filter);
	}
	public async Task<ResponseWrapper<List<PropertyResponseDTO>>> GetAll()
	{
		return await _httpService.Get<ResponseWrapper<List<PropertyResponseDTO>>>("Property/GetList");
	}

	public async Task<ResponseWrapper<PropertyResponseDTO>> GetById(long id)
	{
		return await _httpService.Get<ResponseWrapper<PropertyResponseDTO>>($"Property/Get/{id}");
	}

	public async Task<ResponseWrapper<PropertyResponseDTO>> Add(PropertyDTO propertyDTO)
	{
		return await _httpService.Post<ResponseWrapper<PropertyResponseDTO>>($"Property/Post", propertyDTO);
	}

	public async Task<ResponseWrapper<PropertyResponseDTO>> Update(long id, PropertyDTO propertyDTO)
	{
		return await _httpService.Put<ResponseWrapper<PropertyResponseDTO>>($"Property/Put/{id}", propertyDTO);
	}


    public async Task Delete(long id)
	{
		await _httpService.Delete($"Property/Delete/{id}");
	}

	
}