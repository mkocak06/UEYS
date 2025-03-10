using Core.Entities;
using Core.Models.Authorization;
using Shared.BaseModels;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentResponseDTO = Shared.ResponseModels.ENabiz.StudentResponseDTO;

namespace Core.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        IQueryable<Student> GetWithSubRecords(ZoneModel zone);
        IQueryable<StudentPaginateResponseDTO> OnlyPaginateListQuery(ZoneModel zone);
        IQueryable<BreadCrumbSearchResponseDTO> GetListForBreadCrumb(ZoneModel zone);
        List<CountsByMonthsResponse> GetCountsByMonthsResponse();
        Task<ProtocolProgram> IsProtocolProgram(long programId, CancellationToken cancellationToken);
        IQueryable<StudentChartModel> QueryableStudentsForCharts(ZoneModel zone);
        long StudentCountByInstitution(ZoneModel zone, long parentInstitutionId);
        IQueryable<Student> GetRegistrationStudentQuery(ZoneModel zone);
        Task<List<StudentResponseDTO>> StudentListEnabiz(CancellationToken cancellationToken, DateTime? createdDate);
        IQueryable<StudentExcelExportModel> ExportExcelQuery(ZoneModel zone);
        Task<List<StudentExcelExportModel>> ExportExcelDetailedQuery(ZoneModel zone, ProgramFilter filter);
        Task<List<RestartStudentUserModel>> GetRestartStudents(CancellationToken cancellationToken);
        IQueryable<StudentPaginateResponseDTO> GetExpiredStudents(ZoneModel zone);
        Task DeleteStudent(CancellationToken cancellationToken, EducationTrackingDTO educationTrackingDTO);
        Task UnDeleteStudent(CancellationToken cancellationToken, long id);
        Task<bool> IsActiveStudent(CancellationToken cancellationToken, PlacementExamType sinavTuru, string kimlikNo, DateTime? oncekiSinavTarihi, DateTime? mevcutSinavTarihi);
    }
}
