using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UI.Models;
using System.IO;
using Shared.RequestModels;
using Shared.ResponseModels;
using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using Shared.Types;
using UI.Helper;
using Shared.ResponseModels.Wrapper;
using System.Net.Mime;
using Shared.FilterModels;

namespace UI.Services;

public interface IHttpService
{
    Task<T> Get<T>(string uri);
    Task<T> Post<T>(string uri, object value);
    Task<ResponseWrapper<DocumentResponseDTO>> PostFile(Stream fileStream, string fileName, DocumentTypes documenttype, long entityId);
    Task<string> PostImage(Stream fileStream, string fileName);
    Task<string> PostImageByte(Stream fileStream, string fileName, long userId);
    Task<T> Put<T>(string uri, object value);
    Task<bool> Delete(string uri);
    Task<ResponseWrapper<byte[]>> ExcelByteArray(string uri, object value);
}

public class HttpService : IHttpService
{
    private HttpClient _httpClient;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IConfiguration _configuration;
    private ISweetAlert _sweetAlert;

    public HttpService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService,
        IConfiguration configuration, ISweetAlert sweetAlert)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
        _configuration = configuration;
        _sweetAlert = sweetAlert;
    }

    public async Task<T> Get<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + uri);
        return await sendRequest<T>(request);
    }

    public async Task<T> Post<T>(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest<T>(request);
    }

    public async Task<T> Put<T>(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/" + uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest<T>(request);
    }

    public async Task<bool> Delete(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/" + uri);
        return await sendRequest<bool>(request);
    }
    public async Task<ResponseWrapper<DocumentResponseDTO>> PostFile(Stream fileStream, string fileName, DocumentTypes documenttype, long entityId)
    {
        var baseUrl = _configuration.GetSection("apiUrl").Value;
        var user = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");

        var content = new MultipartFormDataContent();
        content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
        content.Add(new StreamContent(fileStream, (int)fileStream.Length), "File", fileName);
        content.Add(new StringContent(documenttype.ToString()), "DocumentType");
        content.Add(new StringContent(entityId.ToString()), "EntityId");
        using (var client = new HttpClient())
        {
            // Bearer Token header if needed
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

            HttpResponseMessage response = new();
            try
            {
                switch (documenttype)
                {
                    case DocumentTypes.Affiliation:
                        response = await client.PostAsync($"{baseUrl}/api/Affiliation/UploadFile", content);
                        break;
                    case DocumentTypes.ProtocolProgram:
                        response = await client.PostAsync($"{baseUrl}/api/ProtocolProgram/UploadFile", content);
                        break;
                    case DocumentTypes.ComplementProgram:
                        response = await client.PostAsync($"{baseUrl}/api/ProtocolProgram/UploadFile", content);//tamamlayıcı için ayrıca yazmak gerekebilir
                        break;
                    case DocumentTypes.Thesis:
                        response = await client.PostAsync($"{baseUrl}/api/Thesis/UploadFile", content);
                        break;
                    case DocumentTypes.ProgressReport:
                        response = await client.PostAsync($"{baseUrl}/api/ProgressReport/UploadFile", content);
                        break;
                    case DocumentTypes.EthicCommitteeDecision:
                        response = await client.PostAsync($"{baseUrl}/api/EthicCommitteeDecision/UploadFile", content);
                        break;
                    case DocumentTypes.OfficialLetter:
                        response = await client.PostAsync($"{baseUrl}/api/OfficialLetter/UploadFile", content);
                        break;
                    case DocumentTypes.PerformanceRating:
                        response = await client.PostAsync($"{baseUrl}/api/PerformanceRating/UploadFile", content);
                        break;
                    case DocumentTypes.ThesisDefence:
                        response = await client.PostAsync($"{baseUrl}/api/ThesisDefence/UploadFile", content);
                        break;
                    case DocumentTypes.OpinionForm:
                        response = await client.PostAsync($"{baseUrl}/api/OpinionForm/UploadFile", content);
                        break;
                    case DocumentTypes.Communique:
                        response = await client.PostAsync($"{baseUrl}/api/OpinionForm/UploadFile", content);
                        break;
                    case DocumentTypes.EducationTimeTracking:
                        response = await client.PostAsync($"{baseUrl}/api/EducationTracking/UploadFile", content);
                        break;
                    case DocumentTypes.StudentRotation:
                        response = await client.PostAsync($"{baseUrl}/api/StudentRotation/UploadFile", content);
                        break;
                    case DocumentTypes.PlaceOfDuty:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.ScientificStudy:
                        response = await client.PostAsync($"{baseUrl}/api/ScientificStudy/UploadFile", content);
                        break;
                    case DocumentTypes.AssociateProfessorship:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.SpecializationBoardChairman:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.SpecializationBoardMember:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.DeclarationDocument:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.OsymResultDocument:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.Transfer:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.EPKInstitutionalEducationOfficerAppointmentDecision:
                        response = await client.PostAsync($"{baseUrl}/api/User/UploadFile", content);
                        break;
                    case DocumentTypes.EducationOfficerAssignmentLetter:
                        response = await client.PostAsync($"{baseUrl}/api/Educator/UploadFile", content);
                        break;
                    case DocumentTypes.RelatedDependentProgram:
                        response = await client.PostAsync($"{baseUrl}/api/ProtocolProgram/UploadFile", content);
                        break;
                    case DocumentTypes.StudentDependentProgram:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.CommitteeForm or DocumentTypes.TUKForm:
                        response = await client.PostAsync($"{baseUrl}/api/Form/UploadFile", content);
                        break;
                    case DocumentTypes.RegistrationControlForm:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.FeeReceipt:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.PhotocopyOfIdentityCard:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    case DocumentTypes.SpecificEducation:
                        response = await client.PostAsync($"{baseUrl}/api/Student/UploadFile", content);
                        break;
                    default:
                        throw new Exception("Döküman türü bulunamadı. Sistem yöneticisiyle iletişime geçiniz.");
                        break;
                }
                if (response.IsSuccessStatusCode)
                {
                    var uploadedFileWrapper = await response.Content.ReadFromJsonAsync<ResponseWrapper<DocumentResponseDTO>>();
                    //imagePath = $"{url}/{uploadedFileName}";
                    //message = "File has been uploaded successfully!";
                    return new ResponseWrapper<DocumentResponseDTO>() { Item = uploadedFileWrapper.Item, Result = uploadedFileWrapper.Result, Message = uploadedFileWrapper.Message };
                }
                else if (response.StatusCode == HttpStatusCode.PaymentRequired)
                {
                    var uploadedFileWrapper = await response.Content.ReadFromJsonAsync<ResponseWrapper<DocumentResponseDTO>>();
                    //imagePath = $"{url}/{uploadedFileName}";
                    //message = "File has been uploaded successfully!";
                    return new ResponseWrapper<DocumentResponseDTO>() { Message = uploadedFileWrapper.Message, Result = false };
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
        return new();
    }
    public async Task<string> PostImage(Stream fileStream, string fileName)
    {
        var content = new MultipartFormDataContent();
        content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
        content.Add(new StreamContent(fileStream, (int)fileStream.Length), "image", fileName);

        var response = await _httpClient.PostAsync($"/api/FileUpload/PostImage", content);

        if (response.IsSuccessStatusCode)
        {
            var uploadedFileName = await response.Content.ReadAsStringAsync();
            //imagePath = $"{url}/{uploadedFileName}";
            //message = "File has been uploaded successfully!";
            return uploadedFileName;
        }

        return "";
    }
    public async Task<string> PostImageByte(Stream fileStream, string fileName, long userId)
    {
        var baseUrl = _configuration.GetSection("apiUrl").Value;
        var user = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");

        var content = new MultipartFormDataContent();
        content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
        content.Add(new StreamContent(fileStream, (int)fileStream.Length), "File", fileName);
        content.Add(new StringContent(userId.ToString()), "EntityId");

        using (var client = new HttpClient())
        {
            // Bearer Token header if needed
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            var response = await client.PostAsync($"{baseUrl}/api/User/PostImageByte", content);
            if (response.IsSuccessStatusCode)
            {
                var uploadedFile = await response.Content.ReadAsStringAsync();
                //imagePath = $"{url}/{uploadedFileName}";
                //message = "File has been uploaded successfully!";
                return uploadedFile;
            }
        }
        return "";
    }
    // helper methods

    private async Task<T> sendRequest<T>(HttpRequestMessage request)
    {
        // add jwt auth header if user is logged in and request is to the api url
        var user = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");
        var isApiUrl = request.RequestUri != null && !request.RequestUri.IsAbsoluteUri;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        //for api localization
        request.Headers.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.ToString());

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request);

        }
        catch (Exception e)
        {

            throw;
        }
        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("logout");
            return default;
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _sweetAlert.IconAlert(SweetAlertIcon.error, "Giriş Engellendi",
                "Üst üste 10 defa hatalı parola girdiğiniz için 5 dakika boyunca girişiniz engellenmiştir.");
            return default;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<List<ValidationResultModel>>();
            var errors = new List<ValidationFailure>();
            if (result != null)
            {
                errors.AddRange(
                    from validationResultModel in result
                    where validationResultModel.Value != null
                    from validationResultValueModel in validationResultModel.Value
                    select new ValidationFailure(validationResultModel.Key, validationResultValueModel.ErrorMessage)
                    );
            }
            throw new ValidationException(errors);
        }

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error["message"]);
        }

        if (request.Method == HttpMethod.Delete && typeof(T) == typeof(bool))
        {
            return (T)(object)true;
        }

        if (typeof(T) == typeof(String))
            return (T)(object)await response.Content.ReadAsStringAsync();
        else
        {
            var result = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(result))
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else return (T)(object)null;
        }
    }
    public async Task<ResponseWrapper<byte[]>> ExcelByteArray(string uri, object value)
    {
        var baseUrl = _configuration.GetSection("apiUrl").Value;
        var user = await _localStorageService.GetItem<UserForLoginResponseDTO>("user");

        using (var client = new HttpClient())
        {
            // Bearer Token header if needed
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            client.Timeout = new TimeSpan(1, 0, 0);

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/api/{uri}",
                                                                                    new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseWrapper<byte[]>>();
            }
        }
        return null;
    }
}