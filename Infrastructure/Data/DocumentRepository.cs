using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Types;

namespace Infrastructure.Data
{
    public class DocumentRepository : EfRepository<Document>, IDocumentRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Document>> GetByEntityId(CancellationToken cancellationToken, long id, DocumentTypes docType)
        {
            return await GetAsync(cancellationToken, x => x.IsDeleted == false && x.EntityId == id && x.DocumentType == docType);
        }

        public async Task<bool> UpdateSeviceUrl(CancellationToken cancellationToken, long id, string serviceUrl)
        {
            Document document = await GetByIdAsync(cancellationToken, id);

            if (document != null)
            {
                document.ServiceUrl = serviceUrl;

                dbContext.Entry(document).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
