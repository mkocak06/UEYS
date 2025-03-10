using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IRotationService
    {
        Task<PaginationModel<RotationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<RotationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<RotationResponseDTO>>> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId);
        Task<ResponseWrapper<List<CurriculumRotationResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<List<RotationResponseDTO>>> GetFormerStudentListByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<RotationResponseDTO>> PostAsync(CancellationToken cancellationToken, RotationDTO rotationDTO);
        Task<ResponseWrapper<RotationResponseDTO>> Put(CancellationToken cancellationToken, long id, RotationDTO rotationDTO);
        Task<ResponseWrapper<RotationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<RotationResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file);
    }
}
