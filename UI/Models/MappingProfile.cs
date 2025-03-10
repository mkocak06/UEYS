using AutoMapper;
using Newtonsoft.Json;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace UI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<AccountInfoResponseDTO, AccountInfoDTO>();
        CreateMap<AdvisorThesisResponseDTO, AdvisorThesisDTO>();
        CreateMap<AdvisorThesisResponseDTO, ChangeCoordinatorAdvisorThesisDTO>();
        CreateMap<UpdateUserAccountInfoResponseDTO, UpdateUserAccountInfoDTO>();
        CreateMap<UpdateUserAccountInfoResponseDTO, UserForRegisterDTO>();
        CreateMap<UpdateUserAccountInfoDTO, UserAccountDetailInfoResponseDTO>();
        CreateMap<UserAccountDetailInfoResponseDTO, UpdateUserAccountInfoDTO>();
        CreateMap<UserForRegisterDTO, UpdateUserAccountInfoDTO>();
        CreateMap<UserForRegisterDTO, UserAccountDetailInfoResponseDTO>();
        CreateMap<UniversityResponseDTO, UniversityDTO>();
        CreateMap<HospitalResponseDTO, HospitalDTO>();
        CreateMap<ProgramResponseDTO, ProgramDTO>();
        CreateMap<FacultyResponseDTO, FacultyDTO>();
        CreateMap<ProfessionResponseDTO, ProfessionDTO>();
        CreateMap<ExpertiseBranchResponseDTO, ExpertiseBranchDTO>();
        CreateMap<ProvinceResponseDTO, ProvinceDTO>();
        CreateMap<InstitutionResponseDTO, InstitutionDTO>();
        CreateMap<AuthorizationCategoryResponseDTO, AuthorizationCategoryDTO>();
        CreateMap<AuthorizationDetailResponseDTO, AuthorizationDetailDTO>();
        CreateMap<AffiliationResponseDTO, AffiliationDTO>();
        CreateMap<RotationResponseDTO, RotationDTO>();
        CreateMap<ProtocolProgramResponseDTO, ProtocolProgramDTO>();
        CreateMap<DependentProgramResponseDTO, DependentProgramDTO>();
        CreateMap<EducatorExpertiseBranchResponseDTO, EducatorExpertiseBranchDTO>();
        CreateMap<EducatorDependentProgramResponseDTO, EducatorDependentProgramDTO>();
        CreateMap<PerfectionResponseDTO, PerfectionDTO>();
        CreateMap<TitleResponseDTO, TitleDTO>();
        CreateMap<ThesisResponseDTO, ThesisDTO>();
        CreateMap<EducatorResponseDTO, EducatorDTO>();
        CreateMap<EducatorProgramResponseDTO, EducatorProgramDTO>();
        CreateMap<CurriculumResponseDTO, CurriculumDTO>();
        CreateMap<StudentPerfectionResponseDTO, StudentPerfectionDTO>();
        CreateMap<StudentResponseDTO, StudentDTO>();
        CreateMap<StudentRotationResponseDTO, StudentRotationDTO>();
        CreateMap<PerformanceRatingResponseDTO, PerformanceRatingDTO>();
        CreateMap<OpinionFormResponseDTO, OpinionFormDTO>();
        CreateMap<EducationTrackingResponseDTO, EducationTrackingDTO>();
        CreateMap<EthicCommitteeDecisionResponseDTO, EthicCommitteeDecisionDTO>();
        CreateMap<JuryResponseDTO, JuryDTO>();
        CreateMap<OfficialLetterResponseDTO, OfficialLetterDTO>();
        CreateMap<ProgressReportResponseDTO, ProgressReportDTO>();
        CreateMap<DocumentResponseDTO, DocumentDTO>();
        CreateMap<ThesisDefenceResponseDTO, ThesisDefenceDTO>();
        CreateMap<PropertyResponseDTO, PropertyDTO>();
        CreateMap<RoleResponseDTO, RoleDTO>();
        CreateMap<StudentExpertiseBranchResponseDTO, StudentExpertiseBranchDTO>();
        CreateMap<ScientificStudyResponseDTO, ScientificStudyDTO>();
        CreateMap<StudyResponseDTO, StudyDTO>();
        CreateMap<ExitExamResponseDTO, ExitExamDTO>();
        CreateMap<EducatorAdministrativeTitleResponseDTO, EducatorAdministrativeTitleDTO>();
        CreateMap<GraduationDetailResponseDTO, GraduationDetailDTO>();
        CreateMap<RelatedExpertiseBranchResponseDTO, RelatedExpertiseBranchDTO>();
        CreateMap<EducationOfficerResponseDTO, EducationOfficerDTO>();
        CreateMap<EducatorStaffParentInstitutionResponseDTO, EducatorStaffParentInstitutionDTO>();
        CreateMap<EducatorStaffInstitutionResponseDTO, EducatorStaffInstitutionDTO>();
        CreateMap<StudentRotationPerfectionResponseDTO, StudentRotationPerfectionDTO>();
        CreateMap<RelatedDependentProgramResponseDTO, RelatedDependentProgramDTO>();
        CreateMap<UserResponseDTO, UserDTO>();
        CreateMap<StandardCategoryResponseDTO, StandardCategoryDTO>();
        CreateMap<StandardResponseDTO, StandardDTO>();
        CreateMap<FormResponseDTO, FormDTO>();
        CreateMap<FormStandardResponseDTO, FormStandardDTO>();
        CreateMap<OnSiteVisitCommitteeResponseDTO, OnSiteVisitCommitteeDTO>();
        CreateMap<SpecificEducationPlaceResponseDTO, SpecificEducationPlaceDTO>();
        CreateMap<SpecificEducationResponseDTO, SpecificEducationDTO>();
        CreateMap<StudentSpecificEducationResponseDTO, StudentSpecificEducationDTO>();
        CreateMap<AnnouncementResponseDTO, AnnouncementDTO>();

        CreateMap<QuotaRequestResponseDTO, QuotaRequestDTO>();
        CreateMap<SubQuotaRequestResponseDTO, SubQuotaRequestDTO>();
        CreateMap<SubQuotaRequestPaginateResponseDTO, SubQuotaRequestDTO>();
        CreateMap<SubQuotaRequestPortfolioResponseDTO, SubQuotaRequestPortfolioDTO>();
        CreateMap<StudentCountResponseDTO, StudentCountDTO>();
        CreateMap<EducatorCountContributionFormulaResponseDTO, EducatorCountContributionFormulaDTO>();
        CreateMap<PortfolioResponseDTO, PortfolioDTO>();
    }
}