using Core.Models;
using Shared.Models;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICKYSService
    {
        Task<ResponseWrapper<CKYSDoctor>> CKYSDoktorSorgula(string identityNo, bool isBackjob = false);
        Task<ResponseWrapper<GraduationDetailResponseDTO>> GetMedicineInfo(string identityNo);
        Task<CKYSStudent> CKYSOgrenciSorgulaAsync(string identityNo);
        Task<dynamic> CKYSTest(string identityNo);
    }
}
