using Fluxor;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.University;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using UI.Models.FilterModels;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store
{
    public record ProgramDetailState
    {
        public bool ProgramDetailLoading { get; init; }
        public bool ProgramDetailLoaded { get; init; }
        public bool AddProgramDetailLoading { get; init; }
        public bool AddProgramDetailLoaded { get; init; }
        public bool UpdateProgramDetailLoading { get; init; }
        public bool UpdateProgramDetailLoaded { get; init; }
        public ProgramBreadcrumbResponseDTO Program { get; init; }
        public ProgramFilter ProgramFilter { get; set; }
        public DashboardProgramFilter DashboardProgramFilter { get; set; }
    }

    public class ProgramDetailFeature : Feature<ProgramDetailState>
    {
        public override string GetName() => "ProgramDetail";

        protected override ProgramDetailState GetInitialState()
        {
            return new ProgramDetailState
            {
                ProgramDetailLoaded = false,
                ProgramDetailLoading = false,
                UpdateProgramDetailLoading = false,
                UpdateProgramDetailLoaded = false,
                Program = new()
                {
                    RelatedPrograms = new List<ProgramExpertiseBreadcrumbResponseDTO>(),
                    Universities = new List<UniversityBreadcrumbDTO>()
                },
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
                },
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


    public record ProgramDetailLoadAction(long id);
    public record ProgramDetailSetAction(ProgramBreadcrumbResponseDTO ProgramDetail);
    public record ProgramDetailFailureAction(ResponseWrapper<ProgramBreadcrumbResponseDTO> error);
    public record ProgramDetailClearStateAction();
    public record ProgramDetailUpdateAction(long id, ProgramDTO ProgramDetail);
    public record ProgramDetailUpdateSuccessAction(ProgramResponseDTO ProgramDetail);
    public record ProgramDetailUpdateFailureAction(ResponseWrapper<ProgramResponseDTO> error);

    public record ProgramDetailFilterSetAction(ProgramFilter programFilter);
    public record ProgramDetailFilterClearAction();

	public record ProgramDashboardFilterSetAction(DashboardProgramFilter dashboardProgramFilter);
	public record ProgramDashboardFilterClearAction();

	public record ProgramsLoadAction(FilterDTO filter);
}