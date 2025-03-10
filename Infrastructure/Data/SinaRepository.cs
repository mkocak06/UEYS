using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Types;

namespace Infrastructure.Data;

public class SinaRepository : EfRepository<Sina>, ISinaRepository
{
    private readonly ApplicationDbContext dbContext;

    public SinaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task UpdateSinaTable()
    {
        var data = await dbContext.Sina.ToListAsync();
        dbContext.RemoveRange(data);

        var sinaHospitals = (from p in dbContext.Programs.AsNoTracking().Where(x => x.AuthorizationDetails.OrderBy(x => x.AuthorizationDate == null ? 1 : 0)
                                                                              .ThenByDescending(x => x.AuthorizationDate)
                                                                              .FirstOrDefault(x => x.IsDeleted == false).AuthorizationCategory.IsActive == true)
                             let students = p.OriginalStudents.Where(y => !y.IsDeleted && !y.IsHardDeleted && !y.User.IsDeleted)
                                                              .Select(x => x.EducationTrackings.FirstOrDefault(z => z.IsDeleted == false && z.ReasonType == ReasonType.EstimatedFinish).ProcessDate).ToList()
                             let educators = p.EducatorPrograms.Where(x => x.Educator.IsDeleted == false && x.Educator.User.IsDeleted == false &&
                                                                           (x.DutyEndDate == null || DateTime.UtcNow <= x.DutyEndDate))
                             let authorizationCategory = p.AuthorizationDetails.Where(x => x.IsDeleted == false)
                                                                               .OrderBy(x => x.AuthorizationDate == null ? 1 : 0)
                                                                               .ThenByDescending(x => x.AuthorizationDate)
                                                                               .FirstOrDefault().AuthorizationCategory
                             select new Sina()
                             {
                                 IlAdi = p.Hospital.Province.Name,
                                 FakulteTipi = p.Hospital.Faculty.Profession.Name,
                                 UstKurumAdi = p.Hospital.Faculty.University.Institution.Name,
                                 KurumAdi = p.Hospital.Faculty.University.Name,
                                 KurumKodu = p.Hospital.Faculty.University.CKYSCode,
                                 FakulteAdi = p.Hospital.Faculty.Name,
                                 EgitimVerilenKurumAdi = p.Hospital.Name,
                                 EgitimVerilenKurumKodu = p.Hospital.CKYSCode,
                                 BirlikteKullanimYapilanFakulte = p.Faculty.Name,
                                 BirlikteKullanimYapilanKurum = p.Faculty.University.Name,
                                 BirlikteKullanimYapilanKurumKodu = p.Faculty.University.CKYSCode,
                                 UzmanlikDali = p.ExpertiseBranch.Name,
                                 UzmanlikDaliKodu = p.ExpertiseBranch.CKYSCode,
                                 UzmanlikDaliAnaDalMi = p.ExpertiseBranch.IsPrincipal,
                                 YetkiKategorisi = authorizationCategory.Name,
                                 EgiticiSayisi = educators.Count(),
                                 OgrenciMezuniyetTarihiListesi = students,
                             }).OrderBy(x => x.EgitimVerilenKurumAdi).ThenBy(x => x.UzmanlikDali);

        await dbContext.AddRangeAsync(sinaHospitals);
        await dbContext.SaveChangesAsync();
    }
}