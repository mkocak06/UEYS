using Core.Interfaces;
using Core.KDSModels;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ENabizPortfolioRepository : IENabizPortfolioRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ENabizPortfolioRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ENabizPortfolio> GetByAsync(CancellationToken cancellationToken, Expression<Func<ENabizPortfolio, bool>> predicate)
        {
            return await dbContext.ENabizPortfolios.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<ENabizPortfolio>> GetListByAsync(CancellationToken cancellationToken, Expression<Func<ENabizPortfolio, bool>> predicate)
        {
            return await dbContext.ENabizPortfolios.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<List<ENabizPortfolio>> GetListByProgramIdAsync(CancellationToken cancellationToken, long programId, UserType? userType)
        {
            var identityNoList = await UserIdentityNoListByProgramId(cancellationToken, programId, userType);

            return await dbContext.ENabizPortfolios.AsNoTracking().Where(x => identityNoList.Contains(x.hekim_kimlik_numarasi)).ToListAsync(cancellationToken);
        }

        public async Task<ENabizPortfolio> GetTotalOperationsByIdentityNo(CancellationToken cancellationToken, string identityNo, DateTime? startDate, DateTime? endDate)
        {
            return await dbContext.ENabizPortfolios.AsNoTracking()
                                                   .Where(x => x.hekim_kimlik_numarasi == identityNo &&
                                                               (startDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) >= startDate) &&
                                                               (endDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) <= endDate))
                                                   .GroupBy(x => 1)
                                                   .Select(g => new ENabizPortfolio()
                                                   {
                                                       islem_sayisi = g.Sum(x => x.islem_sayisi ?? 0),
                                                       muayene_sayisi = g.Sum(x => x.muayene_sayisi ?? 0),
                                                       ameliyat_ve_girisimler_sayisi = g.Sum(x => x.ameliyat_ve_girisimler_sayisi ?? 0),
                                                       diger_islemler_sayisi = g.Sum(x => x.diger_islemler_sayisi ?? 0),
                                                       dis_islemleri_sayisi = g.Sum(x => x.dis_islemleri_sayisi ?? 0),
                                                       dogum_islemleri_sayisi = g.Sum(x => x.dogum_islemleri_sayisi ?? 0),
                                                       kan_islemleri_sayisi = g.Sum(x => x.kan_islemleri_sayisi ?? 0),
                                                       malzeme_sayisi = g.Sum(x => x.malzeme_sayisi ?? 0),
                                                       tahlil_tetkik_ve_radyoloji_islemleri_sayisi = g.Sum(x => x.tahlil_tetkik_ve_radyoloji_islemleri_sayisi ?? 0),
                                                       yatak_islemleri_sayisi = g.Sum(x => x.yatak_islemleri_sayisi ?? 0),
                                                       a_grubu_ameliyat_sayisi = g.Sum(x => x.a_grubu_ameliyat_sayisi ?? 0),
                                                       b_grubu_ameliyat_sayisi = g.Sum(x => x.b_grubu_ameliyat_sayisi ?? 0),
                                                       c_grubu_ameliyat_sayisi = g.Sum(x => x.c_grubu_ameliyat_sayisi ?? 0),
                                                       d_grubu_ameliyat_sayisi = g.Sum(x => x.d_grubu_ameliyat_sayisi ?? 0),
                                                       e_grubu_ameliyat_sayisi = g.Sum(x => x.e_grubu_ameliyat_sayisi ?? 0),
                                                       recete_sayisi = g.Sum(x => x.recete_sayisi ?? 0),
                                                       ilac_sayisi = g.Sum(x => x.ilac_sayisi ?? 0),
                                                       guncelleme_zamani = g.Max(x => x.guncelleme_zamani)
                                                   })
                                                   .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ENabizPortfolio> GetTotalOperationsByProgramId(CancellationToken cancellationToken, long programId, UserType? userType, DateTime? startDate, DateTime? endDate)
        {
            var identityNoList = await UserIdentityNoListByProgramId(cancellationToken, programId, userType);

            return await dbContext.ENabizPortfolios.AsNoTracking()
                                                   .Where(x => identityNoList.Contains(x.hekim_kimlik_numarasi) &&
                                                               (startDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) >= startDate) &&
                                                               (endDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) <= endDate))
                                                   .GroupBy(x => 1)
                                                   .Select(g => new ENabizPortfolio()
                                                   {
                                                       islem_sayisi = g.Sum(x => x.islem_sayisi ?? 0),
                                                       muayene_sayisi = g.Sum(x => x.muayene_sayisi ?? 0),
                                                       ameliyat_ve_girisimler_sayisi = g.Sum(x => x.ameliyat_ve_girisimler_sayisi ?? 0),
                                                       diger_islemler_sayisi = g.Sum(x => x.diger_islemler_sayisi ?? 0),
                                                       dis_islemleri_sayisi = g.Sum(x => x.dis_islemleri_sayisi ?? 0),
                                                       dogum_islemleri_sayisi = g.Sum(x => x.dogum_islemleri_sayisi ?? 0),
                                                       kan_islemleri_sayisi = g.Sum(x => x.kan_islemleri_sayisi ?? 0),
                                                       malzeme_sayisi = g.Sum(x => x.malzeme_sayisi ?? 0),
                                                       tahlil_tetkik_ve_radyoloji_islemleri_sayisi = g.Sum(x => x.tahlil_tetkik_ve_radyoloji_islemleri_sayisi ?? 0),
                                                       yatak_islemleri_sayisi = g.Sum(x => x.yatak_islemleri_sayisi ?? 0),
                                                       a_grubu_ameliyat_sayisi = g.Sum(x => x.a_grubu_ameliyat_sayisi ?? 0),
                                                       b_grubu_ameliyat_sayisi = g.Sum(x => x.b_grubu_ameliyat_sayisi ?? 0),
                                                       c_grubu_ameliyat_sayisi = g.Sum(x => x.c_grubu_ameliyat_sayisi ?? 0),
                                                       d_grubu_ameliyat_sayisi = g.Sum(x => x.d_grubu_ameliyat_sayisi ?? 0),
                                                       e_grubu_ameliyat_sayisi = g.Sum(x => x.e_grubu_ameliyat_sayisi ?? 0),
                                                       recete_sayisi = g.Sum(x => x.recete_sayisi ?? 0),
                                                       ilac_sayisi = g.Sum(x => x.ilac_sayisi ?? 0),
                                                       guncelleme_zamani = g.Max(x => x.guncelleme_zamani)
                                                   })
                                                   .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ENabizPortfolioChartModel>> OperationsChart(CancellationToken cancellationToken, string identityNo, DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.UtcNow;
            var lastYear = today.AddMonths(-6);

            return await dbContext.ENabizPortfolios.AsNoTracking()
                                                   .Where(x => x.hekim_kimlik_numarasi == identityNo &&
                                                               (startDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) >= startDate) &&
                                                               (endDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) <= endDate))
                                                   .GroupBy(x => new { x.yilay, x.klinik_adi })
                                                   .Select(g => new ENabizPortfolioChartModel()
                                                   {
                                                       KlinikAdi = g.Key.klinik_adi,
                                                       IslemTarihiDate = DateTime.ParseExact(g.Key.yilay, "yyyyMM", null),
                                                       IslemSayisi = g.Sum(x => x.islem_sayisi ?? 0),
                                                       MuayeneSayisi = g.Sum(x => x.muayene_sayisi ?? 0),
                                                       AmeliyatSayisi = g.Sum(x => (x.a_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.b_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.c_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.d_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.e_grubu_ameliyat_sayisi ?? 0)),
                                                   })
                                                   //.Where(g => g.IslemTarihiDate >= lastYear && g.IslemTarihiDate <= today)
                                                   .ToListAsync(cancellationToken);
        }

        public async Task<List<ENabizPortfolioChartModel>> OperationsChartByProgramId(CancellationToken cancellationToken, long programId, UserType? userType, DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.UtcNow;
            var lastYear = today.AddMonths(-6);

            var identityNoList = await UserIdentityNoListByProgramId(cancellationToken, programId, userType);

            return await dbContext.ENabizPortfolios.AsNoTracking()
                                                   .Where(x => identityNoList.Contains(x.hekim_kimlik_numarasi) &&
                                                               (startDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) >= startDate) &&
                                                               (endDate == null || DateTime.ParseExact(x.yilay, "yyyyMM", null) <= endDate))
                                                   .GroupBy(x => new { x.yilay, x.klinik_adi })
                                                   .Select(g => new ENabizPortfolioChartModel()
                                                   {
                                                       KlinikAdi = g.Key.klinik_adi,
                                                       IslemTarihiDate = DateTime.ParseExact(g.Key.yilay, "yyyyMM", null),
                                                       IslemSayisi = g.Sum(x => x.islem_sayisi ?? 0),
                                                       MuayeneSayisi = g.Sum(x => x.muayene_sayisi ?? 0),
                                                       AmeliyatSayisi = g.Sum(x => (x.a_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.b_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.c_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.d_grubu_ameliyat_sayisi ?? 0) +
                                                                                   (x.e_grubu_ameliyat_sayisi ?? 0)),
                                                   })
                                                   //.Where(g => g.IslemTarihiDate >= lastYear && g.IslemTarihiDate <= today)
                                                   .ToListAsync(cancellationToken);
        }

        private async Task<List<string>> UserIdentityNoListByProgramId(CancellationToken cancellationToken, long programId, UserType? userType)
        {
            var identityNoList = new List<string>();

            if (userType == UserType.Student)
            {
                identityNoList = await dbContext.Programs.Where(x => x.Id == programId)
                                                         .SelectMany(x => x.OriginalStudents.Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false)
                                                                                            .Select(x => x.User.IdentityNo)
                                                                          .Concat(x.ProtocolStudents
                                                                                    .Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false)
                                                                                    .Select(s => s.User.IdentityNo)))
                                                         .ToListAsync(cancellationToken);
            }
            else if (userType == UserType.Educator)
            {
                identityNoList = await dbContext.Programs.Where(x => x.Id == programId)
                                                         .SelectMany(x => x.EducatorPrograms.Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false)
                                                                                            .Select(x => x.Educator.User.IdentityNo))
                                                         .ToListAsync(cancellationToken);
            }
            else if (userType == UserType.Specialist)
            {
            }
            else
            {
                identityNoList = await dbContext.Programs.Where(x => x.Id == programId)
                                                         .SelectMany(x => x.OriginalStudents.Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false)
                                                                                            .Select(x => x.User.IdentityNo)
                                                                          .Concat(x.ProtocolStudents
                                                                                    .Where(x => x.IsDeleted == false && x.IsHardDeleted == false && x.User.IsDeleted == false)
                                                                                    .Select(s => s.User.IdentityNo))
                                                                          .Concat(x.EducatorPrograms
                                                                                    .Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false)
                                                                                    .Select(x => x.Educator.User.IdentityNo)))
                                                         .ToListAsync(cancellationToken);
            }

            return identityNoList;
        }
    }
}
