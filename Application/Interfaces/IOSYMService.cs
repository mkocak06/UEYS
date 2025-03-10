using Shared.Models;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Interfaces;

public interface IOSYMService
{
    Task<ResponseWrapper<bool>> IsActiveStudent(CancellationToken cancellationToken, string sinavTuru, string kimlikNo, DateTime? oncekiSinavTarihi, DateTime? mevcutSinavTarihi);
    Task<ResponseWrapper<bool>> AddExamResults(CancellationToken cancellationToken, List<OSYM_StudentExamResult> ogrenciSinavSonuclari);
}