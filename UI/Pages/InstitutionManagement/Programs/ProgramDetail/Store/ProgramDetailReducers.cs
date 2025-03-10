using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.University;
using System.Collections.Generic;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store
{
    public static class ProgramDetailReducers
    {
        [ReducerMethod(typeof(ProgramDetailLoadAction))]
        public static ProgramDetailState OnLoad(ProgramDetailState state)
        {
            return state with
            {
                ProgramDetailLoading = true,
                ProgramDetailLoaded = false
            };
        }
        [ReducerMethod]
        public static ProgramDetailState OnSet(ProgramDetailState state, ProgramDetailSetAction action)
        {
            return state with
            {
                ProgramDetailLoading = false,
                ProgramDetailLoaded = true,
                Program = action.ProgramDetail
            };
        }
        [ReducerMethod]
        public static ProgramDetailState OnFailure(ProgramDetailState state, ProgramDetailFailureAction action)
        {
            return state with
            {
                ProgramDetailLoading = false,
                ProgramDetailLoaded = true,
            };
        }

        [ReducerMethod]
        public static ProgramDetailState OnUpdateLoad(ProgramDetailState state, ProgramDetailUpdateAction action)
        {
            return state with
            {
                UpdateProgramDetailLoading = true,
                UpdateProgramDetailLoaded = false
            };
        }
        [ReducerMethod]
        public static ProgramDetailState OnUpdateSet(ProgramDetailState state, ProgramDetailUpdateSuccessAction action)
        {
            state.Program.Program = action.ProgramDetail;
            return state with
            {
                UpdateProgramDetailLoading = false,
                UpdateProgramDetailLoaded = true,
                Program = state.Program
            };
        }
        [ReducerMethod]
        public static ProgramDetailState OnUpdateFailure(ProgramDetailState state, ProgramDetailUpdateFailureAction action)
        {
            return state with
            {
                UpdateProgramDetailLoading = false,
                UpdateProgramDetailLoaded = true,
            };
        }

        [ReducerMethod]
        public static ProgramDetailState OnClearState(ProgramDetailState state, ProgramDetailClearStateAction action)
        {
            state.Program.Program = null;
            return state with
            {
                ProgramDetailLoaded = false,
                ProgramDetailLoading = false,
                UpdateProgramDetailLoading = false,
                UpdateProgramDetailLoaded = false,
                Program = new()
                {
                    RelatedPrograms = new List<ProgramExpertiseBreadcrumbResponseDTO>(),
                    Universities = new List<UniversityBreadcrumbDTO>()
                }
            };
        }
        
        [ReducerMethod]
        public static ProgramDetailState OnProgramFilterSet(ProgramDetailState state, ProgramDetailFilterSetAction action)
        {
            return state with
            {
                ProgramFilter = action.programFilter
            };
        }
        [ReducerMethod]
        public static ProgramDetailState OnProgramFilterClear(ProgramDetailState state, ProgramDetailFilterClearAction action)
        {
            return state with
            {
                ProgramFilter = new()
                {
                    InstitutionList = new List<InstitutionResponseDTO>(),
                    ExpertiseBranchList = new List<ExpertiseBranchResponseDTO>(),
                    FacultyList = new List<FacultyResponseDTO>(),
                    AuthorizationCategoryList = new List<AuthorizationCategoryResponseDTO>(),
                    ProfessionList = new List<ProfessionResponseDTO>(),
                    HospitalList = new List<HospitalResponseDTO>(),
                    ProvinceList = new List<ProvinceResponseDTO>(),
                    UniversityList = new List<UniversityResponseDTO>()
                }
            };
        }

		[ReducerMethod]
		public static ProgramDetailState OnDashboardProgramFilterSet(ProgramDetailState state, ProgramDashboardFilterSetAction action)
		{
			return state with
			{
				DashboardProgramFilter = action.dashboardProgramFilter
			};
		}
		[ReducerMethod]
		public static ProgramDetailState OnDashboardProgramFilterClear(ProgramDetailState state, ProgramDashboardFilterClearAction action)
		{
			return state with
			{
				DashboardProgramFilter = new()
				{
					InstitutionList = new List<InstitutionResponseDTO>(),
					ExpertiseBranchList = new List<ExpertiseBranchResponseDTO>(),
					FacultyList = new List<FacultyResponseDTO>(),
					AuthorizationCategoryList = new List<AuthorizationCategoryResponseDTO>(),
					ProfessionList = new List<ProfessionResponseDTO>(),
					HospitalList = new List<HospitalResponseDTO>(),
					ProvinceList = new List<ProvinceResponseDTO>(),
					UniversityList = new List<UniversityResponseDTO>(),
					UniversityTypeList = new List<bool?>(),
					BranchTypeList = new List<bool?>(),
				}
			};
		}

	}
}

