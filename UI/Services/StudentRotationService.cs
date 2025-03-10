using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UI.Helper;

namespace UI.Services;

public interface IStudentRotationService
{
    Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetListByStudentId(long studentId);
    Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentListByStudentId(long studentId);
    Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetForEReport(long studentId);
    Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerForEReport(long userId);
    Task<ResponseWrapper<StudentRotationResponseDTO>> GetByIdAsync(long id);
    Task<ResponseWrapper<StudentRotationResponseDTO>> GetEndDateByStartDate(StudentRotationDTO studentRotationDTO);
    Task<ResponseWrapper<StudentRotationResponseDTO>> GetByStudentAndRotationIdAsync(long studentId, long rotationId);
    Task<ResponseWrapper<StudentRotationResponseDTO>> PostAsync(StudentRotationDTO studentRotationDTO);
    Task<ResponseWrapper<StudentRotationResponseDTO>> AddPastRotation(StudentRotationDTO studentRotationDTO);
    Task<ResponseWrapper<StudentRotationResponseDTO>> Update(long id, StudentRotationDTO StudentRotation);
    Task<ResponseWrapper<StudentRotationResponseDTO>> FinishStudentsRotation(long id, StudentRotationDTO studentRotationDTO);
    Task<ResponseWrapper<StudentRotationResponseDTO>> SendStudentToRotation(StudentRotationDTO studentRotationDTO);
    Task<ResponseWrapper<StudentRotationResponseDTO>> Delete(long id);
    Task DeleteActiveRotation(long id);
    Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
    Task DeleteFile(DocumentTypes documentType, long entityId);
}

public class StudentRotationService : IStudentRotationService
{
    private readonly IHttpService _httpService;

    public StudentRotationService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetListByStudentId(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationResponseDTO>>>($"StudentRotation/GetListByStudentId/{studentId}");
    }

    public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentListByStudentId(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationResponseDTO>>>($"StudentRotation/GetFormerStudentListByStudentId/{studentId}");
    }

    public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetForEReport(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationResponseDTO>>>($"EReport/GetRotation/{studentId}");
    }

    public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerForEReport(long userId)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationResponseDTO>>>($"EReport/GetFormerRotation/{userId}");
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/Get/{id}");
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetByStudentAndRotationIdAsync(long studentId, long rotationId)
    {
        return await _httpService.Get<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/GetByStudentAndRotationId?studentId={studentId}&rotationId={rotationId}");
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> PostAsync(StudentRotationDTO Student)
    {
        return await _httpService.Post<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/Post", Student);
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> AddPastRotation(StudentRotationDTO studentRotation)
    {
        return await _httpService.Post<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/AddPastRotation", studentRotation);
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> Update(long id, StudentRotationDTO StudentRotation)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/Put/{id}", StudentRotation);
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> FinishStudentsRotation(long id, StudentRotationDTO studentRotationDTO)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/FinishStudentsRotation/{id}", studentRotationDTO);
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> SendStudentToRotation(StudentRotationDTO studentRotationDTO)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/SendStudentToRotation", studentRotationDTO);
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> Delete(long id)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/Delete/{id}", null);
    }

    public async Task DeleteActiveRotation(long id)
    {
        await _httpService.Delete($"StudentRotation/DeleteActiveRotation/{id}");
    }

    public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
    {
        return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"StudentRotation/DownloadFile/{bucketKey}");
    }
    public async Task DeleteFile(DocumentTypes documentType, long entityId)
    {
        await _httpService.Delete($"StudentRotation/DeleteFileS3?documentType={documentType}&entityId={entityId}");
    }

    public async Task<ResponseWrapper<StudentRotationResponseDTO>> GetEndDateByStartDate(StudentRotationDTO studentRotationDTO)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationResponseDTO>>($"StudentRotation/GetEndDateByStartDate", studentRotationDTO);
    }
}