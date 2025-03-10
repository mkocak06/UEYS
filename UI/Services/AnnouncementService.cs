using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Services;

public interface IAnnouncementService
{
    Task<ResponseWrapper<AnnouncementResponseDTO>> GetById(long id);
    Task<PaginationModel<AnnouncementResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<AnnouncementResponseDTO>> Add(AnnouncementDTO announcement);
    Task<ResponseWrapper<AnnouncementResponseDTO>> Update(long id, AnnouncementDTO announcement);
    Task Delete(long id);
}

public class AnnouncementService : IAnnouncementService
{
    private readonly IHttpService _httpService;

    public AnnouncementService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<AnnouncementResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<AnnouncementResponseDTO>>($"Announcement/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<AnnouncementResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<AnnouncementResponseDTO>>($"Announcement/Get/{id}");
    }

    public async Task<ResponseWrapper<AnnouncementResponseDTO>> Add(AnnouncementDTO announcement)
    {
        return await _httpService.Post<ResponseWrapper<AnnouncementResponseDTO>>($"Announcement/Post", announcement);
    }

    public async Task<ResponseWrapper<AnnouncementResponseDTO>> Update(long id, AnnouncementDTO announcement)
    {
        return await _httpService.Put<ResponseWrapper<AnnouncementResponseDTO>>($"Announcement/Put/{id}", announcement);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Announcement/Delete/{id}");
    }
}