using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPropertyService
    {
        Task<ResponseWrapper<List<PropertyResponseDTO>>> GetByType(CancellationToken cancellationToken, PropertyType propertytype, PerfectionType? perfectionType, string queryText);
        Task<ResponseWrapper<bool>> ImportPropertyList(CancellationToken cancellationToken, IFormFile formFile);
        Task<PaginationModel<PropertyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<PropertyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<PropertyResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<PropertyResponseDTO>> PostAsync(CancellationToken cancellationToken, PropertyDTO propertyDTO);
        Task<ResponseWrapper<PropertyResponseDTO>> Put(CancellationToken cancellationToken, long id, PropertyDTO propertyDTO);
        Task<ResponseWrapper<List<PropertyResponseDTO>>> GetListAsync(CancellationToken cancellationToken);

    }
}
