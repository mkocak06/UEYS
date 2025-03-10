using Core.Entities;
using Microsoft.AspNetCore.Http;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFileManagementService
    {
        Task<ResponseWrapper<DocumentResponseDTO>> UploadFileToS3(CancellationToken cancellationToken, FileUploadModelDTO fileModel);
        Task<ResponseWrapper<FileResponseDTO>> GetFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId);
        Task<ResponseWrapper<bool>> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId);
        Task<ResponseWrapper<FileResponseDTO>> GetFileUrlS3(CancellationToken cancellationToken, long documentId, int expireDay = 7);
        Task<string> PostImageByte(CancellationToken cancellationToken, FileUploadModelDTO fileModel);
    }
}
