using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
    public class EducatorDependentProgramService : BaseService, IEducatorDependentProgramService
    {

        private readonly IMapper mapper;
        private readonly IEducatorDependentProgramRepository educatorDependentProgramRepository;

        public EducatorDependentProgramService(IMapper mapper, IUnitOfWork unitOfWork, IEducatorDependentProgramRepository educatorDependentProgramRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.educatorDependentProgramRepository = educatorDependentProgramRepository;
        }

        public async Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetListByDependProgIdAsync(CancellationToken cancellationToken, long dependProgId)
        {
            List<EducatorDependentProgram> educatorDependentProgram = await educatorDependentProgramRepository.GetListWithSubRecords(cancellationToken, dependProgId);
            List<EducatorDependentProgramResponseDTO> response = mapper.Map<List<EducatorDependentProgramResponseDTO>>(educatorDependentProgram);

            return new ResponseWrapper<List<EducatorDependentProgramResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorDependentProgramDTO educatorDependentProgramDTO)
        {
            EducatorDependentProgram educatorDependentProgram = await educatorDependentProgramRepository.GetIncluding(cancellationToken, x => x.Id == id);

            EducatorDependentProgram updatedEducatorDependentProgram = mapper.Map(educatorDependentProgramDTO, educatorDependentProgram);

            educatorDependentProgramRepository.Update(updatedEducatorDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            EducatorDependentProgramResponseDTO response = mapper.Map<EducatorDependentProgramResponseDTO>(updatedEducatorDependentProgram);

            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorDependentProgramDTO educatorDependentProgramDTO)
        {
            EducatorDependentProgram educatorDependentProgram = mapper.Map<EducatorDependentProgram>(educatorDependentProgramDTO);

            await educatorDependentProgramRepository.AddAsync(cancellationToken, educatorDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<EducatorDependentProgramResponseDTO>(educatorDependentProgram);

            return new ResponseWrapper<EducatorDependentProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long educatorId, long dependentProgramId)
        {
            EducatorDependentProgram educatorDependentProgram = await educatorDependentProgramRepository.GetByAsync(cancellationToken, x => x.EducatorId == educatorId && x.DependentProgramId == dependentProgramId);

            educatorDependentProgramRepository.SoftDelete(educatorDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<EducatorDependentProgramResponseDTO> { Result = true };
        }
    }
}
