using Core.KDSModels;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IENabizPortfolioRepository
    {
        Task<ENabizPortfolio> GetByAsync(CancellationToken cancellationToken, Expression<Func<ENabizPortfolio, bool>> predicate);
        Task<List<ENabizPortfolio>> GetListByAsync(CancellationToken cancellationToken, Expression<Func<ENabizPortfolio, bool>> predicate);
        Task<List<ENabizPortfolio>> GetListByProgramIdAsync(CancellationToken cancellationToken, long programId, UserType? userType);
        Task<ENabizPortfolio> GetTotalOperationsByIdentityNo(CancellationToken cancellationToken, string identityNo, DateTime? startDate, DateTime? endDate);
        Task<ENabizPortfolio> GetTotalOperationsByProgramId(CancellationToken cancellationToken, long programId, UserType? userType, DateTime? startDate, DateTime? endDate);
        Task<List<ENabizPortfolioChartModel>> OperationsChart(CancellationToken cancellationToken, string identityNo, DateTime? startDate, DateTime? endDate);
        Task<List<ENabizPortfolioChartModel>> OperationsChartByProgramId(CancellationToken cancellationToken, long programId, UserType? userType, DateTime? startDate, DateTime? endDate);
    }
}
