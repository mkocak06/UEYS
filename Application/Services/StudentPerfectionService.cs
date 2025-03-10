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
    public class StudentPerfectionService : BaseService, IStudentPerfectionService
    {
        private readonly IMapper mapper;
        private readonly IStudentPerfectionRepository studentPerfectionRepository;
        private readonly IPerfectionService perfectionService;

        public StudentPerfectionService(IMapper mapper, IUnitOfWork unitOfWork, IStudentPerfectionRepository studentPerfectionRepository, IPerfectionService perfectionService) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentPerfectionRepository = studentPerfectionRepository;
            this.perfectionService = perfectionService;
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            StudentPerfection studentPerfection = await studentPerfectionRepository.GetWithSubRecords(cancellationToken, id);
            StudentPerfectionResponseDTO response = mapper.Map<StudentPerfectionResponseDTO>(studentPerfection);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetByStudentIdAsync(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType)
        {
            var studentPerfections = await studentPerfectionRepository.GetListByStudentId(cancellationToken, studentId, perfectionType);

            return new ResponseWrapper<List<StudentPerfectionResponseDTO>> { Result = true, Item = studentPerfections };
        }

        public async Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetListByStudentIdWithoutType(CancellationToken cancellationToken, long studentId)
        {
            var studentPerfections = await studentPerfectionRepository.GetListByStudentIdWithoutType(cancellationToken, studentId);

            return new ResponseWrapper<List<StudentPerfectionResponseDTO>> { Result = true, Item = studentPerfections };
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByStudentAndPerfectionId(CancellationToken cancellationToken, long studentId, long perfectionId)
        {
            StudentPerfection studentPerfection = await studentPerfectionRepository.GetByStudentAndPerfection(cancellationToken, studentId, perfectionId);

            var response = mapper.Map<StudentPerfectionResponseDTO>(studentPerfection);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentPerfectionDTO studentPerfectionDTO)
        {
            StudentPerfection studentPerfection = mapper.Map<StudentPerfection>(studentPerfectionDTO);

            await studentPerfectionRepository.AddAsync(cancellationToken, studentPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentPerfectionResponseDTO>(studentPerfection);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentPerfectionDTO studentPerfectionDTO)
        {
            var studentPerfection = await studentPerfectionRepository.GetByIdAsync(cancellationToken, id);

            StudentPerfection updatedStudentPerfection = mapper.Map(studentPerfectionDTO, studentPerfection);

            studentPerfectionRepository.Update(updatedStudentPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentPerfectionResponseDTO>(updatedStudentPerfection);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long studentId, long perfectionId)
        {
            StudentPerfection studentPerfection = await studentPerfectionRepository.GetByAsync(cancellationToken, x => x.StudentId == studentId && x.PerfectionId == perfectionId && x.IsDeleted == false);

            studentPerfectionRepository.SoftDelete(studentPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> CompleteAllPerfections(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType)
        {
            var perfections = await perfectionService.GetListByStudentId(cancellationToken, studentId, perfectionType);
            var studentPerfections = await GetByStudentIdAsync(cancellationToken, studentId, perfectionType);
            var studentPerfectionIds = studentPerfections.Item.Select(x => x.PerfectionId);

            foreach (var perfection in perfections.Item.Where(x => !studentPerfectionIds.Contains(x.Id)))
            {
                await studentPerfectionRepository.AddAsync(cancellationToken, new StudentPerfection()
                {
                    StudentId = studentId,
                    PerfectionId = perfection.Id,
                    IsSuccessful = true
                });
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentPerfectionResponseDTO> { Result = true };
        }
    }
}
