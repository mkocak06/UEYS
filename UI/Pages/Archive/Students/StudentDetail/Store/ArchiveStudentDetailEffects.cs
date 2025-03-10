using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Services;

namespace UI.Pages.Archive.Students.StudentDetail.Store
{
    public class ArchiveStudentDetailEffects
    {
        private readonly IStudentService StudentService;

        public ArchiveStudentDetailEffects(IStudentService StudentService)
        {
            this.StudentService = StudentService;
        }

        [EffectMethod]
        public async Task LoadStudents(StudentDetailLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<StudentResponseDTO> currentStudent;
            try
            {
                currentStudent = await StudentService.GetById(action.Id,true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                currentStudent = new();
            }
            dispatcher.Dispatch(new StudentDetailSetAction(currentStudent.Item));
        }
        //[EffectMethod]
        //public async Task OnAddNew(StudentDetailAddNewAction action, IDispatcher dispatcher)
        //{
        //    ResponseWrapper<StudentResponseDTO> response;
        //    try
        //    {

        //        response = await StudentService.Add(action.Student);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        currentCurricula = new();
        //    }
        //    dispatcher.Dispatch(new StudentDetailSetAction(currentCurricula.Item));
        //}
    }
}
