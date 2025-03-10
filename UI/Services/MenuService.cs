using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Menu;

namespace UI.Services
{
    public interface IMenuService
    {
        Task<ResponseWrapper<List<MenuResponseDTO>>> GetAll();
        Task<ResponseWrapper<MenuResponseDTO>> GetById(long id);
        Task<ResponseWrapper<MenuResponseDTO>> Add(MenuDTO Menu);
        Task<ResponseWrapper<MenuResponseDTO>> Update(long id, MenuDTO Menu);
        Task Delete(long id);

    }
    public class MenuService : IMenuService
    {
        private readonly IHttpService _httpService;

        public MenuService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<MenuResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<MenuResponseDTO>>>("Menu/GetList");
        }
        public async Task<ResponseWrapper<MenuResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<MenuResponseDTO>>($"Menu/Get/{id}");
        }
        public async Task<ResponseWrapper<MenuResponseDTO>> Add(MenuDTO Menu)
        {
            return await _httpService.Post<ResponseWrapper<MenuResponseDTO>>($"Menu/Post", Menu);
        }
        public async Task<ResponseWrapper<MenuResponseDTO>> Update(long id, MenuDTO Menu)
        {
            return await _httpService.Put<ResponseWrapper<MenuResponseDTO>>($"Menu/Put/{id}", Menu);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"Menu/Delete/{id}");
        }
    }
}
