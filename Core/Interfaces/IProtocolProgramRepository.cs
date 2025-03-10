using Core.Entities;
using Core.Models.Authorization;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProtocolProgramRepository : IRepository<ProtocolProgram>
    {
        Task<List<ProtocolProgramByUniversityIdResponseDTO>> GetListByUniversityId(CancellationToken cancellationToken, long uniId, ProgramType progType);
        Task<ProtocolProgram> GetByIdWithSubRecords(CancellationToken cancellationToken, long id, ProgramType progType);
        Task<ProtocolProgram> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ProtocolProgram protocolProgram);
        Task<List<EducatorDependentProgramResponseDTO>> GetEducatorListForComplementProgram(CancellationToken cancellationToken, long programId);
        IQueryable<ProtocolProgram> PaginateQuery(ZoneModel zone);
    }
}