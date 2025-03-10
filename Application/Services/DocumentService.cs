using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly IMapper mapper;
        private readonly IDocumentRepository documentRepository;

        public DocumentService(IMapper mapper, IUnitOfWork unitOfWork, IDocumentRepository documentRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.documentRepository = documentRepository;
        }
        public async Task<ResponseWrapper<DocumentResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Document document = await documentRepository.GetByIdAsync(cancellationToken, id);
            DocumentResponseDTO response = mapper.Map<DocumentResponseDTO>(document);

            return new ResponseWrapper<DocumentResponseDTO> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListByTypeByEntityAsync(CancellationToken cancellationToken, long entityId, DocumentTypes doctype)
        {
            List<Document> documents = await documentRepository.GetIncludingList(cancellationToken, x => x.EntityId == entityId && x.IsDeleted == false && x.DocumentType == doctype);

            List<DocumentResponseDTO> response = mapper.Map<List<DocumentResponseDTO>>(documents);

            return new ResponseWrapper<List<DocumentResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<DocumentResponseDTO>>> GetDeletedList(CancellationToken cancellationToken)
        {
            List<Document> documents = await documentRepository.GetAsync(cancellationToken,x=>x.IsDeleted == true && x.EntityId != 0);

            List<DocumentResponseDTO> response = mapper.Map<List<DocumentResponseDTO>>(documents);

            return new ResponseWrapper<List<DocumentResponseDTO>> { Result = true, Item = response };
        }
        public async Task<ResponseWrapper<bool>> UnDeleteDocument(CancellationToken cancellationToken, long id)
        {
            var document = await documentRepository.GetByIdAsync(cancellationToken, id);

            if (document != null && document.IsDeleted == true)
            {
                document.IsDeleted = false;
                document.DeleteDate = null;

                documentRepository.Update(document);
                await unitOfWork.CommitAsync(cancellationToken);
                return new ResponseWrapper<bool>() { Result = true };
            }
            else
            {
                return new ResponseWrapper<bool>() { Result = false, Message = "Kayıt bulunamadı!" };
            }
        }
        public async Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            List<Document> documents = await documentRepository.ListAsync(cancellationToken);

            List<DocumentResponseDTO> response = mapper.Map<List<DocumentResponseDTO>>(documents);

            return new ResponseWrapper<List<DocumentResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> UpdateServiceUrl(CancellationToken cancellationToken, long id, string serviceUrl)
        {
            Document document = await documentRepository.GetByIdAsync(cancellationToken, id);

            var updatedDocument = await documentRepository.UpdateSeviceUrl(cancellationToken, id, serviceUrl);

            var response = mapper.Map<DocumentResponseDTO>(updatedDocument);

            return new ResponseWrapper<DocumentResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> PostAsync(CancellationToken cancellationToken, DocumentDTO documentDTO)
        {
            Document document = mapper.Map<Document>(documentDTO);

            await documentRepository.AddAsync(cancellationToken, document);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<DocumentResponseDTO>(document);

            return new ResponseWrapper<DocumentResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> Put(CancellationToken cancellationToken, long id, DocumentDTO documentDTO)
        {
            Document document = await documentRepository.GetByIdAsync(cancellationToken, id);

            Document updatedDocument = mapper.Map(documentDTO, document);

            documentRepository.Update(updatedDocument);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<DocumentResponseDTO>(updatedDocument);

            return new ResponseWrapper<DocumentResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Document document = await documentRepository.GetByIdAsync(cancellationToken, id);

            documentRepository.SoftDelete(document);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<DocumentResponseDTO> { Result = true };
        }
    }
}
