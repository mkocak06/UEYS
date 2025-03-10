using Fluxor;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace UI.Pages.Student.Students.StudentDetail.Store
{
    public static class StudentReducers
    {
        [ReducerMethod]
        public static StudentDetailState OnSet(StudentDetailState state, StudentDetailSetAction action)
        {
            return state with
            {
                Student = action.Student,
                StudentLoading = false,
                StudentLoaded = true
            };
        }
        [ReducerMethod(typeof(StudentDetailLoadAction))]
        public static StudentDetailState OnLoad(StudentDetailState state)
        {
            return state with
            {
                StudentLoading = true,
                StudentLoaded = false
            };
        }
        [ReducerMethod(typeof(StudentClearStateAction))]
        public static StudentDetailState OnClearSet(StudentDetailState state)
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
