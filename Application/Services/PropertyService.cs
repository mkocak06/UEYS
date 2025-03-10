using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Core.Extentsions;
using Shared.RequestModels;
using Shared.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Extentsions;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Types;
using ExcelDataReader;
using Shared.Types;
using Infrastructure.Data;
using Shared.Extensions;
using System.Globalization;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using ExcelDataReader;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class PropertyService : BaseService, IPropertyService
    {
        private readonly IMapper mapper;
        private readonly IPropertyRepository propertyRepository;

        public PropertyService(IMapper mapper, IUnitOfWork unitOfWork, IPropertyRepository propertyRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.propertyRepository = propertyRepository;
        }

        public async Task<ResponseWrapper<List<PropertyResponseDTO>>> GetByType(CancellationToken cancellationToken, PropertyType propertytype, PerfectionType? perfectionType, string queryText)
        {
            var propertyList = new List<Property>();

            if (queryText == null)
            {
                propertyList = await unitOfWork.PropertyRepository.GetAsync(cancellationToken, x => x.PropertyType == propertytype && x.PerfectionType == perfectionType);
            }
            else
            {
                propertyList = await unitOfWork.PropertyRepository.GetAsync(cancellationToken, x => x.PropertyType == propertytype && x.PerfectionType == perfectionType && x.Name.ToLower().Contains(queryText.ToLower()));
            }

            var result = mapper.Map<List<PropertyResponseDTO>>(propertyList);

            return new()
            {
                Item = result,
                Result = true
            };
        }

        public async Task<PaginationModel<PropertyResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Property> ordersQuery = unitOfWork.PropertyRepository.IncludingQueryable(x => true, x => x.PerfectionProperties);
            FilterResponse<Property> filterResponse = ordersQuery.ToFilterView(filter);



            var property = mapper.Map<List<PropertyResponseDTO>>(await filterResponse.Query.ToListAsync());
            var response = new PaginationModel<PropertyResponseDTO>
            {
                Items = property,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
            return response;



        }
        public async Task<ResponseWrapper<List<PropertyResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<Property> query = propertyRepository.Queryable();

            List<Property> properties = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);

            List<PropertyResponseDTO> response = mapper.Map<List<PropertyResponseDTO>>(properties);

            return new ResponseWrapper<List<PropertyResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<PropertyResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Property property = await propertyRepository.GetByIdAsync(cancellationToken, id);

            PropertyResponseDTO response = mapper.Map<PropertyResponseDTO>(property);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PropertyResponseDTO>> Put(CancellationToken cancellationToken, long id, PropertyDTO propertyDTO)
        {
            Property property = await propertyRepository.GetByIdAsync(cancellationToken, id);

            Property updatedProperty = mapper.Map(propertyDTO, property);

            propertyRepository.Update(updatedProperty);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PropertyResponseDTO>(updatedProperty);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PropertyResponseDTO>> PostAsync(CancellationToken cancellationToken, PropertyDTO propertyDTO)
        {
            Property property = mapper.Map<Property>(propertyDTO);

            await propertyRepository.AddAsync(cancellationToken, property);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<PropertyResponseDTO>(property);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<PropertyResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Property property = await propertyRepository.GetByIdAsync(cancellationToken, id);

            propertyRepository.SoftDelete(property);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<bool>> ImportPropertyList(CancellationToken cancellationToken, IFormFile formFile)
        {
            var propertyList = await unitOfWork.PropertyRepository.GetAsync(cancellationToken);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var i = 1;
                    while (reader.Read()) //Each row of the file
                    {
                        if (i == 1)
                        {
                            i++;
                            continue;
                        }

                        var group = reader.GetValue(2)?.ToString().Trim();
                        if (!string.IsNullOrEmpty(group) && !propertyList.Any(x => x.Name == group && x.PropertyType == PropertyType.PerfectionGroup && x.PerfectionType == PerfectionType.Interventional))
                        {

                            propertyList.Add(new Property()
                            {
                                Name = group,
                                PropertyType = PropertyType.PerfectionGroup,
                                PerfectionType = PerfectionType.Interventional
                            });
                        }

                        var name = reader.GetValue(3)?.ToString().Trim();
                        if (!string.IsNullOrEmpty(name) && !propertyList.Any(x => x.Name == name && x.PropertyType == PropertyType.PerfectionName && x.PerfectionType == PerfectionType.Interventional))
                        {
                            propertyList.Add(new Property()
                            {
                                Name = name,
                                PropertyType = PropertyType.PerfectionName,
                                PerfectionType = PerfectionType.Interventional
                            });
                        }

                        var perfectionLevels = reader.GetValue(4)?.ToString().Trim().Split(",");
                        if (perfectionLevels != null)
                        {
                            foreach (var perfectionLevel in perfectionLevels)
                            {
                                if (!string.IsNullOrEmpty(perfectionLevel) && !propertyList.Any(x => x.Name == perfectionLevel.Trim() && x.PropertyType == PropertyType.PerfectionLevel))
                                {
                                    propertyList.Add(new Property()
                                    {
                                        Name = perfectionLevel.Trim(),
                                        PropertyType = PropertyType.PerfectionLevel,
                                    });
                                }
                            }
                        }

                        var seniority = reader.GetValue(5)?.ToString().Trim();
                        if (!string.IsNullOrEmpty(seniority) && !propertyList.Any(x => x.Name == seniority && x.PropertyType == PropertyType.PerfectionSeniorty))
                        {
                            propertyList.Add(new Property()
                            {
                                Name = seniority,
                                PropertyType = PropertyType.PerfectionSeniorty,
                            });
                        }

                        var perfectionMethods = reader.GetValue(6)?.ToString().Trim().Split(",");
                        if (perfectionMethods != null)
                        {
                            foreach (var perfectionMethod in perfectionMethods)
                            {
                                if (!string.IsNullOrEmpty(perfectionMethod) && !propertyList.Any(x => x.Name == perfectionMethod.Trim() && x.PropertyType == PropertyType.PerfectionMethod))
                                {
                                    propertyList.Add(new Property()
                                    {
                                        Name = perfectionMethod.Trim(),
                                        PropertyType = PropertyType.PerfectionMethod,
                                    });
                                }
                            }
                        }
                    }
                }
            }

            var newList = propertyList.Where(x => x.Id == 0).ToList();

            var asdasd = newList.GroupBy(x => new { x.PropertyType, x.Name }).Where(x => x.Count() > 1).Select(x => x.Key.Name).OrderBy(x => x).ToList();

            var y = newList.Where(x => asdasd.Contains(x.Name)).OrderBy(x => x.Name).ToList();


            await unitOfWork.PropertyRepository.AddRangeAsync(cancellationToken, newList);
            await unitOfWork.CommitAsync(cancellationToken);
            return new();
        }
    }
}
