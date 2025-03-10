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
    public interface IStudentPerfectionService
    {
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByStudentAndPerfectionId(CancellationToken cancellationToken, long studentId, long perfectionId);
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentPerfectionDTO studentPerfectionDTO);
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentPerfectionDTO studentRotationDTO);
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long studentId, long perfectionId);
        Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetByStudentIdAsync(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType);
        Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetListByStudentIdWithoutType(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<StudentPerfectionResponseDTO>> CompleteAllPerfections(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType);
    }
}
