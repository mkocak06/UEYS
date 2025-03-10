using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class RelatedDependentProgramService : BaseService, IRelatedDependentProgramService
    {

        private readonly IMapper mapper;

        public RelatedDependentProgramService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.mapper = mapper;
        }


        public async Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, RelatedDependentProgramDTO relatedDependentProgramDTO)
        {
            RelatedDependentProgram relatedDependentProgram = mapper.Map<RelatedDependentProgram>(relatedDependentProgramDTO);

            await unitOfWork.RelatedDependentProgramRepository.AddAsync(cancellationToken, relatedDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<RelatedDependentProgramResponseDTO>(relatedDependentProgram);

            return new ResponseWrapper<RelatedDependentProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, RelatedDependentProgramDTO relatedDependentProgramDTO)
        {
            RelatedDependentProgram relatedDependentProgram = await unitOfWork.RelatedDependentProgramRepository.GetByIdAsync(cancellationToken, id);

            RelatedDependentProgram updatedRelatedDependentProgram = mapper.Map(relatedDependentProgramDTO, relatedDependentProgram);

            unitOfWork.RelatedDependentProgramRepository.Update(updatedRelatedDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<RelatedDependentProgramResponseDTO>(updatedRelatedDependentProgram);

            return new ResponseWrapper<RelatedDependentProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            RelatedDependentProgram relatedDependentProgram = await unitOfWork.RelatedDependentProgramRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.RelatedDependentProgramRepository.SoftDelete(relatedDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<RelatedDependentProgramResponseDTO> { Result = true };
        }
       
    }
}

