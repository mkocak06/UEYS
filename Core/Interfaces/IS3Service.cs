using Core.Entities;
using Microsoft.AspNetCore.Http;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IS3Service
    {
        Task<ResponseWrapper<FileResponseDTO>> GetFileS3(CancellationToken cancellationToken, string bucketKey);
        Task<ResponseWrapper<FileResponseDTO>> GetFileUrlS3(CancellationToken cancellationToken, string bucketKey, int expireDay);
        //Task<string> GetFileS3Url(CancellationToken cancellationToken, Document document, string customFileName = "");
        //Task<List<S3Bucket>> GetS3BucketList(CancellationToken cancellationToken);
        Task<ResponseWrapper<DocumentDTO>> UploadFileToS3(CancellationToken cancellationToken, IFormFile file);
        Task<ResponseWrapper<bool>> DeleteFileS3(CancellationToken cancellationToken, string bucketKey);
        Task<ResponseWrapper<DocumentDTO>> PostImageByte(CancellationToken cancellationToken, IFormFile file);
    }
}
