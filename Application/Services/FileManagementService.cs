using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Security.Claims;

namespace Application.Services
{
    public class FileManagementService : BaseService, IFileManagementService
    {
        private readonly IMapper mapper;
        private readonly IS3Service s3Service;
        private readonly IDocumentRepository documentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FileManagementService(IMapper mapper, IS3Service s3Service, IUnitOfWork unitOfWork, IDocumentRepository documentRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.s3Service = s3Service;
            this.documentRepository = documentRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseWrapper<FileResponseDTO>> GetFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId)
        {
            var document = await documentRepository.FirstOrDefaultAsync(cancellationToken, x => x.DocumentType == documentType && x.EntityId == entityId && x.IsDeleted == false);
            if (document != null)
            {
                var result = await s3Service.GetFileS3(cancellationToken, document.BucketKey);
                result.Item.MimeType = document.MimeType;
                result.Item.Name = document.Name;
                return new() { Result = result.Result, Message = result.Message, Item = result.Item };
            }
            else
            {
                return new() { Message = "Document does not exist", Result = false };
            }
        }

        public async Task<ResponseWrapper<FileResponseDTO>> GetFileUrlS3(CancellationToken cancellationToken, long documentId, int expireDay = 7)
        {
            var document = await documentRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == documentId && x.IsDeleted == false);
            if (document != null)
            {
                var result = await s3Service.GetFileUrlS3(cancellationToken, document.BucketKey, expireDay);
                result.Item.MimeType = document.MimeType;
                result.Item.Name = document.Name;
                return new() { Result = result.Result, Message = result.Message, Item = result.Item };
            }
            else
            {
                return new() { Message = "Document does not exist", Result = false };
            }
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> UploadFileToS3(CancellationToken cancellationToken, FileUploadModelDTO fileModel)
        {
            var result = await s3Service.UploadFileToS3(cancellationToken, fileModel.File);
            if (result.Result)
            {
                result.Item.DocumentType = fileModel.DocumentType;
                result.Item.EntityId = fileModel.EntityId;
                var newDocument = mapper.Map<Core.Entities.Document>(result.Item);

                await documentRepository.AddAsync(cancellationToken, newDocument);
                await unitOfWork.CommitAsync(cancellationToken);

                var response = mapper.Map<DocumentResponseDTO>(newDocument);
                return new() { Result = true, Item = response };
            }
            else
                return new() { Result = false, Message = result.Message };
        }
        public async Task<string> PostImageByte(CancellationToken cancellationToken, FileUploadModelDTO fileModel)
        {
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, fileModel.EntityId);

            var file = fileModel.File;
            if (file == null || file.Length == 0)
                return "";

            string fileName = file.FileName;
            string extension = Path.GetExtension(fileName);

            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

            if (!allowedExtensions.Contains(extension))
                return "";


            string newFileName = $"{Guid.NewGuid()}{extension}";


            string base64 = String.Empty;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                base64 = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
            }
            user.ProfilePhoto = "data:image/png;base64," + base64;

            unitOfWork.UserRepository.Update(user);
            await unitOfWork.CommitAsync(cancellationToken);


            //byte[] b = System.IO.File.ReadAllBytes(filePath);
            return user.ProfilePhoto;

        }

        public async Task<ResponseWrapper<bool>> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId)
        {
            var document = await documentRepository.FirstOrDefaultAsync(cancellationToken, x => x.DocumentType == documentType && x.EntityId == entityId && x.IsDeleted == false);
            if (document != null)
            {
                unitOfWork.DocumentRepository.SoftDelete(document);
                await unitOfWork.CommitAsync(cancellationToken);

                string bucketKey = document.BucketKey;
                var fileResult = await s3Service.DeleteFileS3(cancellationToken, bucketKey);
                return new() { Result = true };
            }
            else
            {
                return new() { Result = false, Message = "Document does not exist" };
            }
        }
    }
}
