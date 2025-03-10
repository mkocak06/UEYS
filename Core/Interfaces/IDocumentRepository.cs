using Core.Entities;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<bool> UpdateSeviceUrl(CancellationToken cancellationToken, long id, string serviceUrl);
        Task<List<Document>> GetByEntityId(CancellationToken cancellationToken, long id, DocumentTypes docType);
    }
}
