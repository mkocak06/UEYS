using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Globalization;

namespace Application.Services
{
    public class CurriculumPerfectionService : BaseService, ICurriculumPerfectionService
    {
        private readonly IMapper mapper;
        private readonly ICurriculumPerfectionRepository curriculumPerfectionRepository;

        public CurriculumPerfectionService(IMapper mapper, IUnitOfWork unitOfWork, ICurriculumPerfectionRepository curriculumPerfectionRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.curriculumPerfectionRepository = curriculumPerfectionRepository;
        }

        public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            CurriculumPerfection curriculumPerfection = await curriculumPerfectionRepository.GetIncluding(cancellationToken, x => x.Id == id && x.IsDeleted == false, x => x.Curriculum, x => x.Perfection);
            CurriculumPerfectionResponseDTO response = mapper.Map<CurriculumPerfectionResponseDTO>(curriculumPerfection);

            return new ResponseWrapper<CurriculumPerfectionResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteCurriculumPerfection(CancellationToken cancellationToken, long id)
        {
            var curriculumPerfection = await curriculumPerfectionRepository.GetByIdAsync(cancellationToken, id);

            if (curriculumPerfection != null && curriculumPerfection.IsDeleted == true)
            {
                curriculumPerfection.IsDeleted = false;
                curriculumPerfection.DeleteDate = null;

                curriculumPerfectionRepository.Update(curriculumPerfection);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<CurriculumPerfection> ordersQuery = curriculumPerfectionRepository.GetWithSubRecords();

            if (filter.Filter.Filters.Any(x => x.Field == "PropertyType"))
            {
                var filteredTypeList = filter.Filter.Filters.Where(x => x.Field == "PropertyType").ToList();
                foreach (var item in filteredTypeList)
                {
                    ordersQuery = ordersQuery.Where(x => x.Perfection.PerfectionProperties.Any(x => x.Property.PropertyType == EnumExtension.ParseEnum<PropertyType>(item.Operator) &&
                                                                                                             (item.Value == null || x.Property.Name.ToLower().Contains(item.Value.ToString().ToLower(new CultureInfo("tr-TR"))))));
                    filter.Filter.Filters.Remove(item);
                }
            }


            FilterResponse<CurriculumPerfection> filterResponse = ordersQuery.ToFilterView(filter);

            var curriculumPerfections = await filterResponse.Query.Select(x => new CurriculumPerfectionResponseDTO()
            {
                Id = x.Id,
                Curriculum = mapper.Map<CurriculumResponseDTO>(x.Curriculum),
                CurriculumId = x.CurriculumId,
                Perfection = new PerfectionResponseDTO
                {
                    Id = x.Perfection.Id,
                    PerfectionType = x.Perfection.PerfectionType,
                    PName = mapper.Map<PropertyResponseDTO>(x.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionName)),
                    Group = mapper.Map<PropertyResponseDTO>(x.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionGroup)),
                    Seniorty = mapper.Map<PropertyResponseDTO>(x.Perfection.PerfectionProperties.Select(x => x.Property).FirstOrDefault(x => x.PropertyType == PropertyType.PerfectionSeniorty)),
                    LevelList = mapper.Map<List<PropertyResponseDTO>>(x.Perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionLevel).ToList()),
                    MethodList = mapper.Map<List<PropertyResponseDTO>>(x.Perfection.PerfectionProperties.Select(x => x.Property).Where(x => x.PropertyType == PropertyType.PerfectionMethod).ToList()),
                },
                PerfectionId = x.PerfectionId
            }).ToListAsync(cancellationToken);

            var response = new PaginationModel<CurriculumPerfectionResponseDTO>
            {
                Items = curriculumPerfections,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> PostAsync(CancellationToken cancellationToken, CurriculumPerfectionDTO curriculumPerfectionDTO)
        {
            CurriculumPerfection curriculumPerfection = mapper.Map<CurriculumPerfection>(curriculumPerfectionDTO);

            var addedPerf = await unitOfWork.PerfectionRepository.AddAsync(cancellationToken, curriculumPerfection.Perfection);
            await unitOfWork.CommitAsync(cancellationToken);
            curriculumPerfection.PerfectionId = addedPerf.Id;

            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.Group.Id, PerfectionId = curriculumPerfection.PerfectionId });
            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.PName.Id, PerfectionId = curriculumPerfection.PerfectionId });
            foreach (var item in curriculumPerfectionDTO.Perfection.LevelList)
            {
                await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = item.Id, PerfectionId = curriculumPerfection.PerfectionId });
            }
            foreach (var item in curriculumPerfectionDTO.Perfection.MethodList)
            {
                await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = item.Id, PerfectionId = curriculumPerfection.PerfectionId });
            }
            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.Seniorty.Id, PerfectionId = curriculumPerfection.PerfectionId });

            await curriculumPerfectionRepository.AddAsync(cancellationToken, curriculumPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<CurriculumPerfectionResponseDTO>(curriculumPerfection);

            return new ResponseWrapper<CurriculumPerfectionResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Put(CancellationToken cancellationToken, long id, CurriculumPerfectionDTO curriculumPerfectionDTO)
        {
            var curriculumPerfection = await curriculumPerfectionRepository.GetIncluding(cancellationToken, x => x.CurriculumId == id && x.PerfectionId == curriculumPerfectionDTO.PerfectionId && x.IsDeleted == false, x => x.Perfection);

            #region DeletePerfectionProperties
            var perfectionProperties = await unitOfWork.PerfectionPropertyRepository.GetAsync(cancellationToken, x => x.PerfectionId == curriculumPerfection.PerfectionId);
            foreach (var item in perfectionProperties)
                unitOfWork.PerfectionPropertyRepository.HardDelete(item);
            #endregion

            #region AddPerfectionProperties
            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.Group.Id, PerfectionId = curriculumPerfection.PerfectionId });
            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.PName.Id, PerfectionId = curriculumPerfection.PerfectionId });
            foreach (var item in curriculumPerfectionDTO.Perfection.LevelList)
            {
                await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = item.Id, PerfectionId = curriculumPerfection.PerfectionId });
            }
            foreach (var item in curriculumPerfectionDTO.Perfection.MethodList)
            {
                await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = item.Id, PerfectionId = curriculumPerfection.PerfectionId });
            }
            await unitOfWork.PerfectionPropertyRepository.AddAsync(cancellationToken, new PerfectionProperty { PropertyId = curriculumPerfectionDTO.Perfection.Seniorty.Id, PerfectionId = curriculumPerfection.PerfectionId });
            #endregion

            var updatedcurriculumPerfection = mapper.Map(curriculumPerfectionDTO, curriculumPerfection);

            curriculumPerfectionRepository.Update(updatedcurriculumPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<CurriculumPerfectionResponseDTO>(updatedcurriculumPerfection);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            CurriculumPerfection curriculumPerfection = await curriculumPerfectionRepository.GetByAsync(cancellationToken, x => x.Id == id && x.IsDeleted == false);

            curriculumPerfectionRepository.SoftDelete(curriculumPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumPerfectionResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> UnDelete(CancellationToken cancellationToken, long id)
        {
            var curriculumPerfection = await curriculumPerfectionRepository.GetByIdAsync(cancellationToken, id);

            curriculumPerfection.IsDeleted = false;
            curriculumPerfection.DeleteDate = null;

            curriculumPerfectionRepository.Update(curriculumPerfection);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<CurriculumPerfectionResponseDTO>() { Result = true };
        }
    }
}
