using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.ResponseModels.Curriculum;
using Shared.Types;

namespace Infrastructure.Data
{
    public class CurriculumRepository : EfRepository<Curriculum>, ICurriculumRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CurriculumRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Curriculum> GetByIdWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await QueryableCurriculums(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Curriculum>> GetListWithSubRecords(CancellationToken cancellationToken)
        {
            return await QueryableCurriculums(x => x.IsDeleted == false).ToListAsync(cancellationToken);
        }

        public IQueryable<Curriculum> QueryableCurriculums(Expression<Func<Curriculum, bool>> predicate)
        {
            return _dbContext.Curricula.AsSplitQuery()
                                       .Include(x => x.ExpertiseBranch) .ThenInclude(x=>x.Profession)
                                       .Where(predicate);
        }

        public async Task<List<CurriculumExportModel>> CurriculumExportModel(CancellationToken cancellationToken)
        {
            var curriculums = await _dbContext.Curricula.AsSplitQuery()
                                                        .Include(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                                                        .Include(x => x.CurriculumRotations.Where(x => x.Rotation.IsDeleted == false))
                                                                                           .ThenInclude(x => x.Rotation)
                                                        .Include(x => x.CurriculumRotations.Where(x => x.Rotation.IsDeleted == false))
                                                                                           .ThenInclude(x => x.Perfections)
                                                                                           .ThenInclude(x => x.PerfectionProperties)
                                                                                           .ThenInclude(x => x.Property)
                                                        .Include(x => x.CurriculumPerfections.Where(x => x.Perfection.IsDeleted == false))
                                                                                             .ThenInclude(x => x.Perfection)
                                                                                             .ThenInclude(x => x.PerfectionProperties)
                                                                                             .ThenInclude(x => x.Property)
                                                        .Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);

            var curriculaModel = new List<CurriculumExportModel>();

            foreach (var curriculum in curriculums)
            {
                var curriculumModel = new CurriculumExportModel()
                {
                    CurriculumName = curriculum.ExpertiseBranch?.Name,
                    CurriculumVersion = curriculum.Version,
                    CurriculumDuration = curriculum.Duration?.ToString(),
                    EffectiveDate = curriculum.EffectiveDate?.ToString("d"),
                    IsActive = curriculum.IsActive == true ? "Aktif" : "Pasif",
                    ProfessionName = curriculum.ExpertiseBranch?.Profession?.Name
                };

                var perfections = curriculum.CurriculumPerfections.Select(x => x.Perfection);
                curriculumModel.Perfections = PerfectionModels(perfections);

                var curriculumRotations = curriculum.CurriculumRotations;

                foreach (var curriculumRotation in curriculumRotations)
                {
                    var rotation = curriculumRotation.Rotation;
                    var rotationPerfections = curriculumRotation.Perfections;

                    var rotationModel = new RotationModel()
                    {
                        RotationName = rotation.ExpertiseBranch != null ? rotation.ExpertiseBranch.Name : "",
                        RotationDuration = rotation.Duration,
                        IsRequired = rotation.IsRequired == true ? "Zorunlu" : "Seçmeli"
                    };

                    rotationModel.Perfections = PerfectionModels(rotationPerfections);

                    curriculumModel.Rotations.Add(rotationModel);
                }

                curriculaModel.Add(curriculumModel);
            }

            return curriculaModel;
        }

        private static List<PerfectionModel> PerfectionModels(IEnumerable<Perfection> perfections)
        {
            var perfectionsModel = new List<PerfectionModel>();

            if (perfections?.Count() > 0)
            {
                foreach (var perfection in perfections)
                {
                    var methods = perfection.PerfectionProperties.Where(x => x.Property.PropertyType == PropertyType.PerfectionMethod).Select(x => x.Property.Name).ToList();
                    var levels = perfection.PerfectionProperties.Where(x => x.Property.PropertyType == PropertyType.PerfectionLevel).Select(x => x.Property.Name).ToList();

                    var curriculumPerfection = new PerfectionModel()
                    {
                        GroupName = perfection.PerfectionProperties.FirstOrDefault(x => x.Property.PropertyType == PropertyType.PerfectionGroup)?.Property?.Name,
                        PerfectionName = perfection.PerfectionProperties.FirstOrDefault(x => x.Property.PropertyType == PropertyType.PerfectionName)?.Property?.Name,
                        SeniorityName = perfection.PerfectionProperties.FirstOrDefault(x => x.Property.PropertyType == PropertyType.PerfectionSeniorty)?.Property?.Name,
                        Methods = String.Join(", ", methods),
                        Levels = String.Join(", ", levels),
                        PerfectionType = perfection.PerfectionType.GetDescription(),
                    };

                    perfectionsModel.Add(curriculumPerfection);
                }
            }

            return perfectionsModel;
        }
    }
}
