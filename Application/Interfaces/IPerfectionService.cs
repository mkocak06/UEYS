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
    public interface IPerfectionService
    {
        Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<PerfectionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<PerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, PerfectionDTO perfectionDTO);
        Task<ResponseWrapper<PerfectionResponseDTO>> PostCurriculumRotationPerfection(CancellationToken cancellationToken, long curriculumId, long rotationId, PerfectionDTO perfectionDTO);
        Task<ResponseWrapper<PerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType);
        Task<ResponseWrapper<PerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, PerfectionDTO perfectionDTO);
        Task<ResponseWrapper<PerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<RotationResponseDTO>> UploadExcel(CancellationToken cancellationToken, IFormFile file);
        Task<ResponseWrapper<bool>> ImportPerfectionList(CancellationToken cancellationToken, IFormFile formFile);
        Task<ResponseWrapper<byte[]>> ExcelExportClinical(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<byte[]>> ExcelExportInterventional(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<bool>> ImportRotationPerfectionList(CancellationToken cancellationToken, IFormFile formFile);
        Task<ResponseWrapper<bool>> CurriculumPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile);
        Task<ResponseWrapper<bool>> CurriculumRotationPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile);
    }
}
