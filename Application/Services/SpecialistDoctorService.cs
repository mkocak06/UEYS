using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.EkipModels;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SpecialistDoctorService : BaseService, ISpecialistDoctorService
    {
        private readonly IMapper mapper;
        private readonly IEkipService ekipService;

        public SpecialistDoctorService(IMapper mapper, IUnitOfWork unitOfWork, IEkipService ekipService) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.ekipService = ekipService;
        }

        public async Task<ResponseWrapper<List<PersonelResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId)
        {
            var program = await unitOfWork.ProgramRepository.GetIncluding(cancellationToken, x => x.Id == programId, x => x.Hospital, x => x.ExpertiseBranch); 

            var specialists = await ekipService.GetListByAsync<Personel>(cancellationToken, x => x.aktif_birim_kod == Convert.ToInt64(program.Hospital.CKYSCode ?? "0") &&
                                                                                                 x.aktif_brans_kod == (program.ExpertiseBranch.CKYSCode ?? "0") && 
                                                                                                 x.aktif_unvan_kod == "8105" && 
                                                                                                 x.calisma_durumu != "Protokol İle Geçici Görevli Çalışıyor");

            List<PersonelResponseDTO> response = mapper.Map<List<PersonelResponseDTO>>(specialists);

            return new ResponseWrapper<List<PersonelResponseDTO>> { Item = response, Result = true };
        }
    }
}
