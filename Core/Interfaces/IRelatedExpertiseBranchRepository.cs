using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRelatedExpertiseBranchRepository
    {
        Task AddAsync(CancellationToken cancellationToken, RelatedExpertiseBranch relatedExpertiseBranch);
    }
}
