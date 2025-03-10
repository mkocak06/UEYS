using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class DependentProgramService : BaseService, IDependentProgramService
    {
        private readonly IDependentProgramRepository dependentProgramRepository;
        private readonly IMapper mapper;
        public DependentProgramService(IUnitOfWork unitOfWork, IDependentProgramRepository dependentProgramRepository, IMapper mapper) : base(unitOfWork)
        {
            this.dependentProgramRepository = dependentProgramRepository;
            this.mapper = mapper;
        }
        public async Task<ResponseWrapper<DependentProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, DependentProgramDTO dependentProgramDTO)
        {
            DependentProgram dependentProgram = mapper.Map<DependentProgram>(dependentProgramDTO);

            await dependentProgramRepository.AddAsync(cancellationToken, dependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<DependentProgramResponseDTO>(dependentProgram);

            return new ResponseWrapper<DependentProgramResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<DependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, DependentProgramDTO dependentProgramDTO)
        {
            DependentProgram dependentProgram = await dependentProgramRepository.GetByIdAsync(cancellationToken, id);

            DependentProgram updatedDependentProgram = mapper.Map(dependentProgramDTO, dependentProgram);

            dependentProgramRepository.Update(updatedDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<DependentProgramResponseDTO>(updatedDependentProgram);

            return new ResponseWrapper<DependentProgramResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<DependentProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            DependentProgram dependentProgram = await dependentProgramRepository.GetByIdAsync(cancellationToken, id);

            dependentProgramRepository.HardDelete(dependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<DependentProgramResponseDTO> { Result = true };
        }
    }
}
