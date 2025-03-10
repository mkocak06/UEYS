using Core.Entities;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IYOKService
    {
        Task<ResponseWrapper<dynamic>> GetMezunAsync(string identityNo);
        Task<ResponseWrapper<dynamic>> GetSaglikMezunAsync(int gun, int ay, int yil);
        AcademicAdminStaffResponseDTO GetAkademikIdariPersonelGorevAsync(long? tckNo);
        Task<ResponseWrapper<dynamic>> GetAkademisyenGorevlendirmeAsync(long? tckNo);
        Task<ResponseWrapper<dynamic>> GetAkademisyenUzmanlikAsync(long? tckNo);
        Task<ResponseWrapper<dynamic>> GetIdariPersonelAsync(long? tckNo);
        Task<List<GraduationDetailResponseDTO>> GetEgitimBilgisiAsync(long tc);

    }
}
