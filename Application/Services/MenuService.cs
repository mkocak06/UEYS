using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.Menu;
using Microsoft.AspNetCore.Http;
using Koru;
using Koru.Interfaces;

namespace Application.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IMapper _mapper;
        private readonly IMenuRepository menuRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;
        public MenuService(IUnitOfWork unitOfWork, IMapper mapper, IMenuRepository menuRepository, IHttpContextAccessor httpContextAccessor, IKoruRepository koruRepository) : base(unitOfWork)
        {
            _mapper = mapper;
            this.menuRepository = menuRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.koruRepository = koruRepository;
        }

        public async Task<ResponseWrapper<List<MenuResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Menu> menus = await menuRepository.ListAsync(cancellationToken);

            List<MenuResponseDTO> response = _mapper.Map<List<MenuResponseDTO>>(menus);
            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<MenuResponseDTO>>> GetListByUser(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);

            var roles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, userId);
            List<Menu> menus = await menuRepository.GetListByRoleId(cancellationToken, user.SelectedRoleId);

            List<MenuResponseDTO> response = _mapper.Map<List<MenuResponseDTO>>(menus);
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<MenuResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Menu menu = await menuRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == id);

            MenuResponseDTO response = _mapper.Map<MenuResponseDTO>(menu);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<MenuResponseDTO>> PostAsync(CancellationToken cancellationToken, MenuDTO menuDTO)
        {
            Menu menu = _mapper.Map<Menu>(menuDTO);

            await unitOfWork.MenuRepository.AddAsync(cancellationToken, menu);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<MenuResponseDTO>(menu);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<MenuResponseDTO>> Put(CancellationToken cancellationToken, long id, MenuDTO menuDTO)
        {
            Menu menu = await menuRepository.GetByIdAsync(cancellationToken, id);

            Menu updatedMenu = _mapper.Map(menuDTO, menu);

            unitOfWork.MenuRepository.Update(updatedMenu);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<MenuResponseDTO>(updatedMenu);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<MenuResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Menu menu = await menuRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.MenuRepository.SoftDelete(menu);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}