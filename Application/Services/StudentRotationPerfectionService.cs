using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StudentRotationPerfectionService : BaseService, IStudentRotationPerfectionService
    {
        private readonly IMapper mapper;
        private readonly IStudentRotationPerfectionRepository studentRotationPerfectionRepository;

        public StudentRotationPerfectionService(IMapper mapper, IUnitOfWork unitOfWork, IStudentRotationPerfectionRepository studentRotationPerfectionRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentRotationPerfectionRepository = studentRotationPerfectionRepository;
        }
        public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> GetByStrIdAndPerfectionId(CancellationToken cancellationToken, long studentRotationId, long perfectionId)
        {
            StudentRotationPerfection studentRotationPerfection = await studentRotationPerfectionRepository.GetIncluding(cancellationToken, x => x.PerfectionId == perfectionId && x.StudentRotationId == studentRotationId, x => x.StudentRotation, x => x.Perfection, x => x.Educator.User);
            var response = mapper.Map<StudentRotationPerfectionResponseDTO>(studentRotationPerfection);

            return new ResponseWrapper<StudentRotationPerfectionResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentRotationPerfectionDTO studentRotationPerfectionDTO)
        {
            StudentRotationPerfection studentRotationPerfection = mapper.Map<StudentRotationPerfection>(studentRotationPerfectionDTO);

            await studentRotationPerfectionRepository.AddAsync(cancellationToken, studentRotationPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentRotationPerfectionResponseDTO>(studentRotationPerfection);

            return new ResponseWrapper<StudentRotationPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentRotationPerfectionDTO studentRotationPerfectionDTO)
        {
            var StudentRotationPerfection = await studentRotationPerfectionRepository.GetByIdAsync(cancellationToken, id);

            StudentRotationPerfection updatedStudentRotationPerfection = mapper.Map(studentRotationPerfectionDTO, StudentRotationPerfection);

            studentRotationPerfectionRepository.Update(updatedStudentRotationPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentRotationPerfectionResponseDTO>(updatedStudentRotationPerfection);

            return new ResponseWrapper<StudentRotationPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            StudentRotationPerfection StudentRotationPerfection = await studentRotationPerfectionRepository.GetByIdAsync(cancellationToken, id);

            studentRotationPerfectionRepository.SoftDelete(StudentRotationPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentRotationPerfectionResponseDTO> { Result = true };
        }

    }
}
