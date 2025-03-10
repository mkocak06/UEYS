using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Shared.ResponseModels.ProtocolProgram;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace Application.Interfaces
{
    public interface IEducationTrackingService
    {
        Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetListByStudentIdAsync(CancellationToken cancellationToken, long studentId);
        Task<PaginationModel<EducationTrackingResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> GetById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> PostAsync(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> Put(CancellationToken cancellationToken, long id, EducationTrackingDTO educationTrackingDTO);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> ReturnToMainProgramInProtocol(CancellationToken cancellationToken, long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgramDTO);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> SendStudentToDependentProgram(CancellationToken cancellationToken, long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgramDTO);
        Task<ResponseWrapper<int>> GetRemainingDaysForDependentProgram(CancellationToken cancellationToken, StudentDependentProgramPaginateDTO studentDependentProgramDTO);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> GetEducationStartByStudentId(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetTimeIncreasingRecordsByDate(CancellationToken cancellationToken, OpinionFormRequestDTO opinionForm);
    }
}
