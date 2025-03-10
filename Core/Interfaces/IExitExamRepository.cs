using Core.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Core.Interfaces
{
    public interface IExitExamRepository : IRepository<ExitExam>
    {
        Task<ExitExam> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ExitExam exitExam);
        IQueryable<ExitExam> QueryableExitExam(Expression<Func<ExitExam, bool>> predicate);
        public Task<ExitExam> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<Student> GetStudentWithSubRecords(CancellationToken cancellationToken, long studentId);
    }
}
