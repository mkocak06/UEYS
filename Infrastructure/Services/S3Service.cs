using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Core.Entities;
using Core.Extentsions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.ConfigModels;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Infrastructure.Services
{
    public class S3Service : IS3Service
    {
        private readonly AppSettingsModel appSettingsModel;
        //private readonly IAmazonS3 amazonS3Client;
        private readonly ILogger<S3Service> logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AmazonS3Client amazonS3Client;
        public S3Service(AppSettingsModel appSettingsModel, ILogger<S3Service> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.appSettingsModel = appSettingsModel;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            amazonS3Client = GetClient();
        }
        private AmazonS3Client GetClient()
        {
            AmazonS3Config config = new()
            {
                ServiceURL = appSettingsModel.S3.ServiceURL,
                UseHttp = appSettingsModel.S3.SSL,
                SignatureVersion = appSettingsModel.S3.SignatureVersion,
                ForcePathStyle = appSettingsModel.S3.ForcePathStyle
            };

            string awsAccessKey = appSettingsModel.S3.AccessKey;
            string awsSecretKey = appSettingsModel.S3.SecretKey;

            return new AmazonS3Client(awsAccessKey, awsSecretKey, config);
        }
        public async Task<ResponseWrapper<DocumentDTO>> PostImageByte(CancellationToken cancellationToken, IFormFile file)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            logger.LogInformation("{0} - {1} PostImageByte.", userId, file.FileName);

            if (file == null || file.Length == 0)
                return new() { Result = false, Message = "Upload a file" };


            string fileName = file.FileName;
            string contentType = MimeTypeHelpers.GetMimeTypeForFileExtension(fileName);
            //string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

            //if (!allowedExtensions.Contains(contentType))
            //    return new() { Result = false, Message = "File is not a valid image" };

            using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);

            var bucketKey = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            PutObjectRequest request = new()
            {
                InputStream = newMemoryStream,
                BucketName = appSettingsModel.S3.BucketName,
                Key = bucketKey,
                ContentType = contentType
            };

            var result = await amazonS3Client.PutObjectAsync(request, cancellationToken);

            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                DocumentDTO documentDTO = new() { ServiceUrl = appSettingsModel.S3.ServiceURL, MimeType = contentType, Name = file.FileName, BucketKey = bucketKey, DocumentType = DocumentTypes.Image };
                return new() { Result = true, Item = documentDTO };
            }
            else
            {
                return new() { Result = false, Message = "Error encountered while writing an object" };
            }
        }
        public async Task<ResponseWrapper<DocumentDTO>> UploadFileToS3(CancellationToken cancellationToken, IFormFile file)
        {
            if (file.Length > 3145728)
            {
                return new() { Result = false, Message = "File size is over the limit." };
            }
            var userId = httpContextAccessor.HttpContext.GetUserId();
            logger.LogInformation("{0} - {1} UploadFileToS3.", userId, file.FileName);

            string contentType = MimeTypeHelpers.GetMimeTypeForFileExtension(file.FileName);
            if (!MimeType.ValidTypes.Contains(contentType))
                return new() { Result = false, Message = "İzin verilmeyen dosya uzantısı" };

            using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);
            //TO DO file name'in nasıl olacağı
            var bucketKey = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            PutObjectRequest request = new()
            {
                InputStream = newMemoryStream,
                BucketName = appSettingsModel.S3.BucketName,
                Key = bucketKey,
                ContentType = contentType
            };

            var result = await amazonS3Client.PutObjectAsync(request, cancellationToken);

            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                DocumentDTO documentDTO = new() { ServiceUrl = appSettingsModel.S3.ServiceURL, MimeType = contentType, Name = file.FileName, BucketKey = bucketKey };
                return new() { Result = true, Item = documentDTO };
            }
            else
            {
                return new() { Result = false, Message = "Error encountered while writing an object" };
            }
        }
        public async Task<ResponseWrapper<FileResponseDTO>> GetFileS3(CancellationToken cancellationToken, string bucketKey)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            logger.LogInformation("{0} - {1} GetFileS3.", userId, bucketKey);

            GetObjectRequest request = new()
            {
                BucketName = appSettingsModel.S3.BucketName,
                Key = bucketKey
            };

            GetObjectResponse response = await amazonS3Client.GetObjectAsync(request, cancellationToken);

            Stream stream = response.ResponseStream;

            if (stream != null)
            {
                var fileContent = ByteHelpers.ReadAllBytes(stream);


                FileResponseDTO fileResponse = new()
                {
                    FileContent = fileContent
                };

                return new() { Result = true, Item = fileResponse };
            }
            else
            {
                return new() { Result = false, Message = "Document does not exist in Repo" };
            }
        }
        public async Task<ResponseWrapper<FileResponseDTO>> GetFileUrlS3(CancellationToken cancellationToken, string bucketKey, int expireDay = 7)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            logger.LogInformation("{0} - {1} GetFileS3.", userId, bucketKey);


            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = appSettingsModel.S3.BucketName,
                Key = bucketKey,
                Expires = DateTime.Now.AddDays(expireDay),
                Protocol = Protocol.HTTPS
            };

            var fileS3Url = amazonS3Client.GetPreSignedURL(urlRequest);

            if (!string.IsNullOrEmpty(fileS3Url))
            {
                FileResponseDTO fileResponse = new()
                {
                    FileUrl = fileS3Url
                };

                return new() { Result = true, Item = fileResponse };
            }
            else
            {
                return new() { Result = false, Message = "Document does not exist in Repo" };
            }
        }
        public async Task<ResponseWrapper<bool>> DeleteFileS3(CancellationToken cancellationToken, string bucketKey)
        {
            var userId = httpContextAccessor.HttpContext.GetUserId();
            logger.LogInformation("{0} - {1} DeleteFileS3.", userId, bucketKey);

            DeleteObjectRequest deleteObjectRequest = new()
            {
                BucketName = appSettingsModel.S3.BucketName,
                Key = bucketKey
            };
            DeleteObjectResponse result = await amazonS3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);

            return new() { Result = true, Item = true };
        }

    }
}
