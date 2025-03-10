using Application.Interfaces;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.RequestModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationDetailController : ControllerBase
    {
        private readonly IAuthorizationDetailService authorizationDetailService;
        private readonly IAuthorizationDetailRepository authorizationDetailRepository;
        private readonly IUnitOfWork unitOfWork;
        public AuthorizationDetailController(IAuthorizationDetailService authorizationDetailService, IAuthorizationDetailRepository authorizationDetailRepository, IUnitOfWork unitOfWork)
        {
            this.authorizationDetailService = authorizationDetailService;
            this.authorizationDetailRepository = authorizationDetailRepository;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("{programId}")]
        //[HasPermission(Shared.Types.PermissionEnum.AuthorizationDetailGetListByProgram)]
        public async Task<IActionResult> GetListByProgramId(CancellationToken cancellationToken, long programId) => Ok(await authorizationDetailService.GetListByProgramId(cancellationToken, programId));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationDetailGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await authorizationDetailService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationDetailAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] AuthorizationDetailDTO authorizationDetailDTO) => Ok(await authorizationDetailService.PostAsync(cancellationToken, authorizationDetailDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationDetailUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] AuthorizationDetailDTO authorizationDetailDTO) => Ok(await authorizationDetailService.Put(cancellationToken, id, authorizationDetailDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationDetailDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await authorizationDetailService.Delete(cancellationToken, id));

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> PutDescriptions(CancellationToken cancellationToken)
        {
            var auths = await authorizationDetailRepository.GetAsync(cancellationToken);
            foreach (var auth in auths)
            {
                auth.Description = JsonConvert.SerializeObject(new List<string>() {auth.Description});
                authorizationDetailRepository.Update(auth);
            }
            await unitOfWork.CommitAsync(cancellationToken);
            return Ok(true);
        }
    }
}
