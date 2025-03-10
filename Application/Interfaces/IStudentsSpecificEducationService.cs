using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStudentsSpecificEducationService
    {
        Task<ResponseWrapper<List<StudentSpecificEducationResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<PaginationModel<StudentSpecificEducationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentSpecificEducationDTO studentsSpecificEducationDTO);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentSpecificEducationDTO studentsSpecificEducationDTO);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
    }
}
