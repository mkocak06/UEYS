using Application.Interfaces;
using Application.Models;
using Application.Reports.ExcelReports.UserReports;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Entities.Koru;
using Core.Extentsions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Core.Models.ConfigModels;
using Core.UnitOfWork;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.FilterModels.Base;
using Shared.Models.SMSModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Authorization;
using Shared.ResponseModels.Wrapper;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IMapper mapper;
        private readonly IAuthRepository authRepository;
        private readonly IKoruRepository koruRepository;
        private readonly IUserRepository userRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly ICryptoService cryptoService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJWTTokenService tokenService;
        private readonly IStringLocalizer<Shared.ApiResource> sharedResource;
        private readonly AppSettingsModel appSettingsModel;
        private readonly IHttpClientFactory clientFactory;
        private readonly IEmailSender emailSender;

        public AuthService(IAuthRepository authRepository, IMapper mapper,
            IKoruRepository koruRepository, IUnitOfWork unitOfWork,
            IUserRepository userRepository, ICryptoService cryptoService,
            IHttpContextAccessor httpContextAccessor, IJWTTokenService tokenService,
            IStringLocalizer<Shared.ApiResource> sharedResource, IHttpClientFactory clientFactory, AppSettingsModel appSettingsModel, IDocumentRepository documentRepository, IEmailSender emailSender) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.authRepository = authRepository;
            this.koruRepository = koruRepository;
            this.userRepository = userRepository;
            this.cryptoService = cryptoService;
            this.httpContextAccessor = httpContextAccessor;
            this.tokenService = tokenService;
            this.sharedResource = sharedResource;
            this.clientFactory = clientFactory;
            this.appSettingsModel = appSettingsModel;
            this.documentRepository = documentRepository;
            this.emailSender = emailSender;
        }
        public string ResourceExample(UserForLoginDTO userForLoginDTO)
        {
            var _shared = sharedResource["Example"].Value;
            var shared = Shared.Resources.ApiResource.Example;
            return _shared;
        }
        public async Task<ResponseWrapper<UserForRegisterDTO>> Create(CancellationToken cancellationToken, UserForRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.Email = userForRegisterDTO.Email.Trim().ToLower(); //Convert username to lower case before storing in database.

            if (await authRepository.UserExists(cancellationToken, userForRegisterDTO.Email))
                return new ResponseWrapper<UserForRegisterDTO> { Result = false, Message = "Email is already taken" };
            User user = mapper.Map<User>(userForRegisterDTO);
            await authRepository.Create(cancellationToken, user);

            return new ResponseWrapper<UserForRegisterDTO> { Result = true, Item = userForRegisterDTO };
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> Login(CancellationToken cancellationToken, UserForLoginDTO userForLoginDTO)
        {
            var userResponse = await authRepository.Login(cancellationToken, userForLoginDTO.Email.ToLower(), userForLoginDTO.Password);
            if (userResponse.Result)
            {
                var response = tokenService.GenerateToken(userResponse.Item.Id, userResponse.Item.Email);
                userResponse.Item.Token = response.Token;
                userResponse.Item.TokenExpiration = response.Expiration;
            }

            return userResponse;
        }
        public async Task<ResponseWrapper<UserForLoginResponseDTO>> OGNLogin(CancellationToken cancellationToken, string identityNo)
        {
            var userResponse = await authRepository.LoginWithIdentityNo(cancellationToken, identityNo);

            if (userResponse.Result)
            {
                var response = tokenService.GenerateToken(userResponse.Item.Id, userResponse.Item.Email);
                userResponse.Item.Token = response.Token;
                userResponse.Item.TokenExpiration = response.Expiration;
            }

            return userResponse;
        }

        public async Task<ResponseWrapper<bool>> SendVerificationMessage(CancellationToken cancellationToken, string phone)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var user = await userRepository.GetByIdAsync(cancellationToken, userId);
            Random random = new Random();
            var verificationCode = random.Next(100000, 999999);
            var model = new SMSModel() { PhoneNumber = phone.ToString(), SmsContent = $"Doğrulama kodu : {verificationCode}" };
            user.PhoneVerificationCode = verificationCode;
            user.Phone = phone.ToString();
            await userRepository.SendMessage(model);
            userRepository.Update(user);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<bool>> SendVerificationMail(CancellationToken cancellationToken, string email)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var user = await userRepository.GetByIdAsync(cancellationToken, userId);
            Random random = new Random();
            var verificationCode = random.Next(100000, 999999);
            EmailMessage model = new() { To = [email], Subject = "Doğrulama Kodu", Body = $"Doğrulama Kodu: {verificationCode}" };
            emailSender.SendEmail(model);
            user.MailVerificationCode = verificationCode;
            user.Email = email.ToString();
            userRepository.Update(user);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<bool>> VerifyPhone(CancellationToken cancellationToken, int verificationCode)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var user = await userRepository.GetByIdAsync(cancellationToken, userId);
            if (user.PhoneVerificationCode == verificationCode)
            {
                user.IsPhoneVerified = true;
                userRepository.Update(user);
            }
            return new() { Result = user.PhoneVerificationCode == verificationCode };
        }

        public async Task<ResponseWrapper<bool>> VerifyMail(CancellationToken cancellationToken, int verificationCode)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var user = await userRepository.GetByIdAsync(cancellationToken, userId);
            if (user.MailVerificationCode == verificationCode)
            {
                user.IsMailVerified = true;
                userRepository.Update(user);
            }
            return new() { Result = user.MailVerificationCode == verificationCode };
        }

        public ICollection<PermissionDTO> Permissions()
        {
            ICollection<Permission> permissions = koruRepository.GetPermissions();
            var permissionsDTOs = mapper.Map<ICollection<Permission>, ICollection<PermissionDTO>>(permissions);
            return permissionsDTOs;
        }

        public async Task<List<RoleResponseDTO>> Roles(CancellationToken cancellationToken)
        {
            var roles = await koruRepository.GetRolesAsync(cancellationToken);
            var roleDTOs = mapper.Map<List<Role>, List<RoleResponseDTO>>(roles);
            return roleDTOs;
        }
        public async Task<List<RoleResponseDTO>> GetRolesByUserId(CancellationToken cancellationToken, long userId)
        {
            var roles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, userId);
            var roleDTOs = mapper.Map<List<Role>, List<RoleResponseDTO>>(roles);
            return roleDTOs;
        }
        public async Task<ResponseWrapper<UserRoleResponseDTO>> CreateUserRoles(CancellationToken cancellationToken, UserRoleDTO userRolesDTO)
        {
            await koruRepository.CreateUserRolesAsync(cancellationToken, userRolesDTO.UserId, userRolesDTO.RoleIds);
            return new ResponseWrapper<UserRoleResponseDTO> { Result = true };
        }

        public async Task<ResponseWrapper<bool>> UpdateUserRoles(CancellationToken cancellationToken, UserRoleDTO userRoleDTO)
        {
            await koruRepository.UpdateUserRolesAsync(cancellationToken, userRoleDTO.UserId, userRoleDTO.RoleIds);
            return new ResponseWrapper<bool> { Result = true };
        }

        public async Task<RoleResponseDTO> AddRole(CancellationToken cancellationToken, RoleDTO role)
        {
            Role newRole = await koruRepository.AddRoleAsync(cancellationToken, role.RoleName, role.Description);
            var homepageMenu = await koruRepository.GetMenuByMenuNameAsync(cancellationToken, MenuConstants.HomePage);
            if (homepageMenu != null)
            {
                await koruRepository.AddMenuToRole(cancellationToken, newRole.Id, homepageMenu.Id); //Homepage added as default
            }
            var roleDTO = mapper.Map<Role, RoleResponseDTO>(newRole);
            return roleDTO;
        }

        public async Task<ResponseWrapper<bool>> UpdateRolePermissions(CancellationToken cancellationToken, RolePermissionDTO rolePermissionDTO)
        {
            await koruRepository.UpdateRolePermissionsAsync(cancellationToken, rolePermissionDTO.RoleId, rolePermissionDTO.Permissions);
            return new ResponseWrapper<bool> { Result = true };
        }

        public async Task<ResponseWrapper<AccountInfoResponseDTO>> CreateUserAccount(CancellationToken cancellationToken, UserForRegisterDTO userForRegisterDTO)
        {

            if (await authRepository.UserExists(cancellationToken, userForRegisterDTO.Email.Trim().ToLower()))
                return new ResponseWrapper<AccountInfoResponseDTO> { Result = false, Message = "Email is already taken" };


            string password = cryptoService.CreatePassword(4);
            cryptoService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new()
            {
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            user = mapper.Map<User>(userForRegisterDTO);
            await unitOfWork.UserRepository.AddAsync(cancellationToken, user);
            await unitOfWork.CommitAsync(cancellationToken);

            await userRepository.SendWelcomeMessage(user.Phone);

            //if (userForRegisterDTO.RoleIds != null && userForRegisterDTO.RoleIds.Count > 0)
            //{
            //    await koruRepository.UpdateUserRolesAsync(cancellationToken, user.Id, userForRegisterDTO.RoleIds);
            //}
            if (userForRegisterDTO.Zones != null && userForRegisterDTO.Zones.Count > 0)
            {
                await koruRepository.UpdateUserRolesAndZonesAsync(cancellationToken, user.Id, userForRegisterDTO.Zones);
            }
            if (userForRegisterDTO.Documents?.Count > 0)
            {
                foreach (var item in userForRegisterDTO.Documents)
                {
                    item.EntityId = user.Id;
                    documentRepository.Update(mapper.Map<Document>(item));
                }
            }

            return new() { Result = true };
        }

        public async Task<ResponseWrapper<List<UserAccountDetailInfoResponseDTO>>> GetUserAccountList(CancellationToken cancellationToken)
        {
            var list = await userRepository.GetList(cancellationToken);
            return new() { Result = true, Item = mapper.Map<List<UserAccountDetailInfoResponseDTO>>(list) };
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> UpdateUserAccount(CancellationToken cancellationToken, long userId, UpdateUserAccountInfoDTO updateUserAccount)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            //updateUserAccount.IdentityNo = user.IdentityNo;
            var updatedUser = mapper.Map(updateUserAccount, user);
            updatedUser.UpdateDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateUserAccount.Password))
            {
                cryptoService.CreatePasswordHash(updateUserAccount.Password, out byte[] passwordHash, out byte[] passwordSalt);

                updatedUser.PasswordHash = passwordHash;
                updatedUser.PasswordSalt = passwordSalt;
            }
            if (updateUserAccount.Zones != null && updateUserAccount.Zones.Count > 0)
            {

                if (!updateUserAccount.Zones.Select(x => x.RoleId).ToList().Contains(user.SelectedRoleId))
                    updatedUser.SelectedRoleId = updateUserAccount.Zones.FirstOrDefault().RoleId;
                await koruRepository.UpdateUserRolesAndZonesAsync(cancellationToken, user.Id, updateUserAccount.Zones);
                //await UpdateUserRoles(cancellationToken, new UserRoleDTO() { RoleIds = updateUserAccount.RoleIds, UserId = updateUserAccount.Id });
            }

            unitOfWork.UserRepository.Update(updatedUser);
            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true, Item = mapper.Map<UserAccountDetailInfoResponseDTO>(updatedUser) };
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> ChangeUserPassword(CancellationToken cancellationToken, ChangePasswordDTO changePassword)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            var user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            bool oldPasswordCurrect = cryptoService.VerifyPassword(changePassword.CurrentPassword, user.PasswordHash, user.PasswordSalt);

            if (!oldPasswordCurrect)
            {
                return new() { Result = false, Message = "Wrong old password" };
            }

            cryptoService.CreatePasswordHash(changePassword.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = tokenService.GenerateToken(user.Id, user.Email);

            return new() { Result = true, Item = new() { Email = user.Email, Name = user.Name, Token = response.Token, TokenExpiration = response.Expiration } };
        }
        public async Task<PaginationModel<UserPaginateResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            var ordersQuery = unitOfWork.UserRepository.PaginateQuery(zone);


            var roleFilter = filter.Filter.Filters.FirstOrDefault(x => x.Field == "Role");
            if (roleFilter != null)
            {
                    ordersQuery = ordersQuery.Where(x => x.Roles.Contains((string)roleFilter.Value));
                    filter.Filter.Filters.Remove(roleFilter);
            }

            var workingPlaceFilter = filter.Filter?.Filters?.FirstOrDefault(x => x.Field == "WorkingPlace");
            if (workingPlaceFilter?.Value?.ToString() != null)
            {
                var filterValue = workingPlaceFilter.Value.ToString()?.ToLower() ?? "";

                ordersQuery = ordersQuery.Where(x => x.HospitalZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                                     x.FacultyZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                                     x.UniversityZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                                     x.ProgramZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                                     x.ProvinceZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                                     x.StudentZone.ToLower().Contains(filterValue));

                filter.Filter.Filters.Remove(workingPlaceFilter);
            }

            var filterResponse = ordersQuery.ToFilterView(filter);

            var users = await filterResponse.Query.ToListAsync(cancellationToken);
            users = users.DistinctBy(x => x.Id).ToList();
            if (users?.Count > 0)
            {
                foreach (var item in users)
                {
                    item.IdentityNo = !string.IsNullOrWhiteSpace(item.IdentityNo) ? StringHelpers.MaskIdentityNumber(item.IdentityNo) : "10*******70";

                    // Servisten çekerken tolower ve totitleCase yapıldığında filtre için yapılan tolower fonksiyonu sorguyu bozuyor. O yüzden burada yapıldı.
                    //item.StudentZone = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.StudentZone?.ToLower() ?? "");
                    //item.EducatorZone = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.EducatorZone?.ToLower() ?? "");
                }
            }

            var response = new PaginationModel<UserPaginateResponseDTO>
            {
                Items = users,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<UpdateUserAccountInfoResponseDTO>> UnDeleteUser(CancellationToken cancellationToken, long id)
        {
            var user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, id);

            if (await unitOfWork.UserRepository.AnyAsync(cancellationToken, x => x.IdentityNo == user.IdentityNo && x.IsDeleted == false))
                return new ResponseWrapper<UpdateUserAccountInfoResponseDTO>() { Result = false, Message = "Aktif etmek istediğiniz TCKN'ye ait aktif kayıt bulunmaktadır!" };

            user.IsDeleted = false;
            user.DeleteDate = null;

            unitOfWork.UserRepository.Update(user);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<UpdateUserAccountInfoResponseDTO>() { Result = true };
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetByIdAsync(CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            if (userId == 0) return new();
            User user = await userRepository.GetByIdWithSubRecords(cancellationToken, userId);
            UserAccountDetailInfoResponseDTO response = mapper.Map<UserAccountDetailInfoResponseDTO>(user);
            response.IdentityNo = StringHelpers.MaskIdentityNumber(response.IdentityNo);
            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> Delete(CancellationToken cancellationToken, long id)
        {
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, id);

            unitOfWork.UserRepository.SoftDelete(user);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<bool> { Result = true };
        }

        public async Task<ResponseWrapper<List<PermissionDTO>>> GetUserPermissionsModelAsync(CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var result = await koruRepository.GetUserPermissionsModelAsync(cancellationToken, userId, user.SelectedRoleId);
            return new() { Result = true, Item = mapper.Map<List<PermissionDTO>>(result) };
        }

        public async Task<ResponseWrapper<List<PermissionDTO>>> AddPermissionToRole(CancellationToken cancellationToken, RolePermission2DTO rolePermissionDTO)
        {
            var rolePermission = mapper.Map<RolePermission>(rolePermissionDTO);

            await koruRepository.AddPermissionToRole(cancellationToken, rolePermission);

            return new() { Result = true };
        }
        public async Task<ResponseWrapper<List<PermissionDTO>>> RemovePermissionToRole(CancellationToken cancellationToken, RolePermission2DTO rolePermissionDTO)
        {
            var rolePermission = mapper.Map<RolePermission>(rolePermissionDTO);

            await koruRepository.RemovePermissionToRole(cancellationToken, rolePermission);

            return new() { Result = true };
        }
        public async Task<ResponseWrapper<RoleResponseDTO>> UpdateRole(CancellationToken cancellationToken, long id, string roleName, string description)
        {
            var role = await koruRepository.UpdateRoleAsync(cancellationToken, id, roleName, description);
            var response = mapper.Map<RoleResponseDTO>(role);

            return new() { Result = true, Item = response };
        }
        public async Task<bool> UpdateSelectedRole(CancellationToken cancellationToken, long selectedRoleId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.GetUserId();
                User user = await unitOfWork.UserRepository.GetIncluding(cancellationToken, x => x.Id == userId, x => x.UserRoles);

                if (user.UserRoles?.Any(x => x.RoleId == selectedRoleId) == true)
                {
                    user.SelectedRoleId = selectedRoleId;

                    unitOfWork.UserRepository.Update(user);
                    await unitOfWork.CommitAsync(cancellationToken);

                    return true;
                }
                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<ResponseWrapper<RoleResponseDTO>> RemoveRole(CancellationToken cancellationToken, long id)
        {
            try
            {
                await koruRepository.RemoveRole(cancellationToken, id);
                return new() { Result = true };
            }
            catch (Exception ex)
            {
                return new() { Result = false, Message = ex.Message };
            }
        }

        public async Task<ResponseWrapper<RoleResponseDTO>> GetRoleById(CancellationToken cancellationToken, long id)
        {
            try
            {
                var role = await koruRepository.GetRoleById(cancellationToken, id);

                var response = mapper.Map<Role, RoleResponseDTO>(role);

                return new() { Result = true, Item = response };
            }
            catch (Exception ex)
            {
                return new() { Result = false, Message = ex.Message };
            }
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;

            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);
            var ordersQuery = unitOfWork.UserRepository.PaginateQuery(zone);

            var roleFilter = filter.Filter.Filters.Where(x => x.Field == "Role");
            if (roleFilter != null)
            {
                foreach (var role in roleFilter.ToList())
                {
                    ordersQuery = ordersQuery.Where(x => x.Roles.Contains((string)role.Value));
                    filter.Filter.Filters.Remove(role);
                }
            }

            var workingPlaceFilter = filter.Filter?.Filters?.FirstOrDefault(x => x.Field == "WorkingPlace");
            if (workingPlaceFilter?.Value?.ToString() != null)
            {
                var filterValue = workingPlaceFilter.Value.ToString()?.ToLower() ?? "";

                ordersQuery = ordersQuery.Where(x => x.HospitalZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                       x.FacultyZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                       x.UniversityZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                       x.ProgramZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                       x.ProvinceZones.Any(x => x.ToLower().Contains(filterValue)) ||
                                       x.EducatorZone.ToLower().Contains(filterValue) ||
                                       x.StudentZone.ToLower().Contains(filterValue));

                filter.Filter.Filters.Remove(workingPlaceFilter);
            }

            var filterResponse = ordersQuery.ToFilterView(filter);

            var users = mapper.Map<List<UserPaginateResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<UserPaginateResponseDTO>
            {
                Items = users,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            var byteArray = ExportList.ExportListReport(response.Items);

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }
        /// <summary>
        /// kullanıcının atayabileceği rolleri getirir
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseWrapper<List<RoleResponseDTO>>> GetRolesByUserRole(CancellationToken cancellationToken)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var userRoles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, userId);
            var roles = await koruRepository.GetRolesByUserRole(cancellationToken, userRoles);

            var response = mapper.Map<List<Role>, List<RoleResponseDTO>>(roles);

            return new() { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<ZoneModelDTO>> GetZone(CancellationToken cancellationToken, long roleId)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            var userRoles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, userRoles, roleId);

            var response = mapper.Map<ZoneModelDTO>(zone);

            return new() { Result = true, Item = response };
        }

        public async Task<bool> AssignSelectedRoles(CancellationToken cancellationToken)
        {
            var users = await unitOfWork.UserRepository.GetIncludingList(cancellationToken, x => true, x => x.UserRoles);

            foreach (var user in users.Where(x => x.UserRoles?.Any(x => x.RoleId != null) == true))
            {
                user.SelectedRoleId = user.UserRoles.FirstOrDefault(x => x.RoleId != null).RoleId.Value;
                unitOfWork.UserRepository.Update(user);
            }
            await unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        #region field
        //public async Task<ResponseWrapper<List<FieldResponseDTO>>> GetFieldByUser(CancellationToken cancellationToken)
        //{
        //    long userId = httpContextAccessor.HttpContext.GetUserId();
        //    var roles = await koruRepository.GetRolesByUserIdAsync(cancellationToken, userId);
        //    List<Core.Entities.Koru.Field> fields;
        //    if (role.CategoryId == (int)RoleCategoryType.Admin)
        //        fields = await unitOfWork.FieldRepository.ListAsync(cancellationToken);
        //    else
        //        fields = await unitOfWork.FieldRepository.GetListByRoleId(cancellationToken, role.Id);
        //    return new() { Result = true, Item = mapper.Map<List<FieldResponseDTO>>(fields) };
        //}
        //public async Task<ResponseWrapper<bool>> AddFieldToRole(CancellationToken cancellationToken, RoleFieldDTO roleFieldDTO)
        //{
        //    RoleField roleField = new() { FieldId = roleFieldDTO.FieldId, RoleId = roleFieldDTO.RoleId };
        //    await unitOfWork.RoleFieldRepository.AddAsync(cancellationToken, roleField);
        //    await unitOfWork.CommitAsync(cancellationToken);
        //    return new() { Result = true, Item = true };
        //}
        //public async Task<ResponseWrapper<bool>> RemoveFieldFromRole(CancellationToken cancellationToken, RoleFieldDTO roleFieldDTO)
        //{
        //    var result = await unitOfWork.RoleFieldRepository.FirstOrDefaultAsync(cancellationToken, x => x.FieldId == roleFieldDTO.FieldId && x.RoleId == roleFieldDTO.RoleId);
        //    unitOfWork.RoleFieldRepository.HardDelete(result);
        //    await unitOfWork.CommitAsync(cancellationToken);
        //    return new() { Result = true, Item = true };
        //}
        #endregion
        #region roleMenu
        public async Task<ResponseWrapper<bool>> AddMenuToRole(CancellationToken cancellationToken, long roleId, long menuId)
        {
            var response = await koruRepository.AddMenuToRole(cancellationToken, roleId, menuId);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<bool>> RemoveMenuToRole(CancellationToken cancellationToken, long roleId, long menuId)
        {
            await koruRepository.RemoveMenuToRole(cancellationToken, roleId, menuId);
            return new() { Result = true };
        }

        #endregion
        #region OGN
        public string RedirectSSO()
        {
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            return $"{appSettingsModel.SSO.OpenIdServer}/connect/authorize?client_id={appSettingsModel.SSO.ClientId}&redirect_uri={appSettingsModel.SSO.RedirectUri}&response_type=code&code_challenge={codeChallenge}&code_challenge_method=S256&State={codeVerifier}&scope={appSettingsModel.SSO.Scope}";
        }

        public string Logout()
        {
            return $"{appSettingsModel.SSO.OpenIdServer}/Account/LogOutWithRedirectUrl?redirectUrl={appSettingsModel.SSO.LogoutUri}";
        }

        public async Task<ResponseWrapper<UserForLoginResponseDTO>> LoginOpenId(CancellationToken cancellationToken, SsoLoginDTO ssoLoginDTO)
        {
            var ognToken = await GetToken(ssoLoginDTO.Code, ssoLoginDTO.State);
            var userInfo = await GetUserInfo(ognToken.AccessToken);

            var user = await OGNLogin(cancellationToken, userInfo.IdentityNumber);

            return user;
        }

        private async Task<OGNUserInfoModel> GetUserInfo(string AccessToken)
        {
            var client = clientFactory.CreateClient("ssoopenid");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            var result = await client.GetAsync("connect/userinfo");
            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<OGNUserInfoModel>(resultContent);
                return userInfo;
            }
            else
            {
                throw new Exception("User Info servisi hata ile karşılaştı");
            }
        }

        private async Task<OGNTokenModel> GetToken(string Code, string State)
        {
            var content = new FormUrlEncodedContent(new[]
             {
                new KeyValuePair<string, string>("client_id", appSettingsModel.SSO.ClientId),
                new KeyValuePair<string, string>("client_secret", appSettingsModel.SSO.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", appSettingsModel.SSO.RedirectUri),
                new KeyValuePair<string, string>("code", Code),
                new KeyValuePair<string, string>("code_verifier", State),

                });

            var client = clientFactory.CreateClient("ssoopenid");
            //client.BaseAddress = new Uri(appSettingsModel.SSO.OpenIdServer);
            var result = await client.PostAsync("/connect/token", content);
            if (result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<OGNTokenModel>(resultContent);
                return token;
            }
            else
            {
                throw new Exception("Token servisi hata ile karşılaştı");
            }
        }

        private string GenerateCodeVerifier()
        {
            var rng = RandomNumberGenerator.Create();

            var bytes = new byte[32];
            rng.GetBytes(bytes);

            // It is recommended to use a URL-safe string as code_verifier.
            // See section 4 of RFC 7636 for more details.
            var code_verifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return code_verifier;
        }

        private string GenerateCodeChallenge(string CodeVerifier)
        {
            var code_challenge = string.Empty;
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(CodeVerifier));
                code_challenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

                return code_challenge;
            }
        }
        #endregion
    }
}
