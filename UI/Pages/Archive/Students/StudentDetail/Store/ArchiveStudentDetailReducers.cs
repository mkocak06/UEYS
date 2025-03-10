using Fluxor;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace UI.Pages.Archive.Students.StudentDetail.Store
{
    public static class ArchiveStudentDetailReducers
    {
        [ReducerMethod]
        public static ArchiveStudentDetailState OnSet(ArchiveStudentDetailState state, StudentDetailSetAction action)
        {
            return state with
            {
                Student = action.Student,
                StudentLoading = false,
                StudentLoaded = true
            };
        }
        [ReducerMethod(typeof(StudentDetailLoadAction))]
        public static ArchiveStudentDetailState OnLoad(ArchiveStudentDetailState state)
        {
            return state with
            {
                StudentLoading = true,
                StudentLoaded = false
            };
        }
        [ReducerMethod(typeof(StudentClearStateAction))]
        public static ArchiveStudentDetailState OnClearSet(ArchiveStudentDetailState state)
        {
            return state with
            {
                StudentLoading = false,
                StudentLoaded = false,
                Student = new StudentResponseDTO(),
            };
        }
    }
}
