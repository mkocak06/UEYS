using Application.Interfaces;
using Application.Services.Base;
using Core.UnitOfWork;
using Shared.Extensions;
using Shared.Models;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services;

public class OSYMService : BaseService, IOSYMService
{
    public OSYMService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<ResponseWrapper<bool>> IsActiveStudent(CancellationToken cancellationToken, string sinavTuru, string kimlikNo, DateTime? oncekiSinavTarihi, DateTime? mevcutSinavTarihi)
    {
        try
        {
            var sinavTuruEnum = PlacementExamType.TUS;

            if (!string.IsNullOrWhiteSpace(sinavTuru))
            {
                sinavTuruEnum = EnumExtension.ToEnum<PlacementExamType>(sinavTuru);
            }

            var response = await unitOfWork.StudentRepository.IsActiveStudent(cancellationToken, sinavTuruEnum, kimlikNo, oncekiSinavTarihi, mevcutSinavTarihi);
            return new ResponseWrapper<bool>() { Result = true, Message = "Ýþlem Baþarýlý.", Item = response };
        }
        catch (Exception ex)
        {

            return new ResponseWrapper<bool>() { Result = false, Message = "Ýþlem Baþarýsýz."};
        }
    }

    public async Task<ResponseWrapper<bool>> AddExamResults(CancellationToken cancellationToken, List<OSYM_StudentExamResult> ogrenciSinavSonuclari)
    {
        return new();
    }
} 