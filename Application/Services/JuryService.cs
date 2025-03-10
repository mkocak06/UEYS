using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru;
using Koru.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class JuryService : BaseService, IJuryService
    {
        private readonly IMapper mapper;
        private readonly IJuryRepository juryRepository;
        private readonly IKoruRepository koruRepository;

        public JuryService(IMapper mapper, IUnitOfWork unitOfWork, IJuryRepository juryRepository, IKoruRepository koruRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.juryRepository = juryRepository;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<JuryResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Jury jury = await juryRepository.GetWithSubRecords(cancellationToken, id);
            JuryResponseDTO response = mapper.Map<JuryResponseDTO>(jury);

            return new ResponseWrapper<JuryResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<JuryResponseDTO>> PostAsync(CancellationToken cancellationToken, JuryDTO juryDTO)
        {
            if (juryDTO.Zone != null)
            {
                await koruRepository.AddStudentZoneToUserRole(cancellationToken, juryDTO.Zone.UserId, juryDTO.Zone.StudentId, RoleConstants.JURY);
            }
            Jury jury = mapper.Map<Jury>(juryDTO);

            await juryRepository.AddAsync(cancellationToken, jury);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<JuryResponseDTO>(jury);

            return new ResponseWrapper<JuryResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<JuryResponseDTO>> Delete(CancellationToken cancellationToken, long educatorId, long thesisId)
        {
            Jury jury = await juryRepository.GetByAsync(cancellationToken, x => x.EducatorId == educatorId && x.ThesisDefenceId == thesisId && x.IsDeleted == false);

            juryRepository.SoftDelete(jury);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<JuryResponseDTO> { Result = true };
        }
    }
}
