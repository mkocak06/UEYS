using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBackgroundJobService
    {
        Task CheckNotLoggedUsers();
        Task UpdateSinaTable();
        Task UpdateEducatorType();
        Task MakeUserPassive();
        Task CheckExpiredStudents();
        Task SystemLogInformation();
        Task UpdateMvTcknViewAsync();
        Task CheckEducatorProgramsEndDate();
        Task CheckSpecialistStudents();
        Task CheckEducatorProgramsEndDateSendWarning();
    }
}
