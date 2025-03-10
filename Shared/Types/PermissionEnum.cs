using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum PermissionEnum
    {
        #region Authorization
        [PermissionInformation("Create new user", "Authorization")]
        CreateUser,
        [PermissionInformation("Delete new user", "Authorization")]
        DeleteUser,
        [PermissionInformation("User excel export", "Authorization")]
        UserExcelExport,
        [PermissionInformation("User permission list", "Authorization")]
        GetUserPermissionsModel,
        [PermissionInformation("List all roles", "Authorization")]
        ListRoles,
        [PermissionInformation("List all permissions", "Authorization")]
        ListPermissions,
        [PermissionInformation("Add new role", "Authorization")]
        AddRole,
        [PermissionInformation("Update role", "Authorization")]
        UpdateRole,
        [PermissionInformation("Add permission to role", "Authorization")]
        AddPermissionToRole,
        [PermissionInformation("Remove permission to role", "Authorization")]
        RemovePermissionToRole,
        [PermissionInformation("Delete role", "Authorization")]
        DeleteRole,
        [PermissionInformation("Update user roles", "Authorization")]
        UpdateUserRoles,
        [PermissionInformation("Create user account", "Authorization")]
        CreateUserAccount,
        [PermissionInformation("Get user list", "Authorization")]
        GetUserList,
        //[PermissionInformation("Get user by id", "Authorization")]
        //GetUserById,
        [PermissionInformation("Get user account detail", "Authorization")]
        GetUserAccountDetail,
        [PermissionInformation("Update user account", "Authorization")]
        UpdateUserAccount,
        [PermissionInformation("Update user own account password", "Authorization")]
        UpdateUserOwnAccountPassword,
        [PermissionInformation("AuthorizationDetail add menu to role", "Authorization")]
        AuthorizationAddMenuToRole,
        [PermissionInformation("AuthorizationDetail remove menu to role", "Authorization")]
        AuthorizationRemoveMenuToRole,
        [PermissionInformation("AuthorizationDetail get roles by user role", "Authorization")]
        AuthorizationGetRolesByUserRole,
        [PermissionInformation("AuthorizationDetail get role by id", "Authorization")]
        AuthorizationGetRoleById,
        [PermissionInformation("AuthorizationDetail get roles by user role", "Authorization")]
        AuthorizationGetZone,
        [PermissionInformation("AuthorizationDetail get fields by user", "Authorization")]
        AuthorizationGetFieldsByUser,
        [PermissionInformation("AuthorizationDetail add field to role", "Authorization")]
        AuthorizationAddFieldToRole,
        [PermissionInformation("AuthorizationDetail remove filed from role", "Authorization")]
        AuthorizationRemoveFieldFromRole,
        #endregion

        #region User
        [PermissionInformation("User Get By Identity No", "User")]
        UserGetByIdentityNo,
        [PermissionInformation("User Add User With EducatorInfo", "User")]
        UserAddUserWithEducatorInfo,
        [PermissionInformation("User Is Existing User", "User")]
        IsExistingUser,
        [PermissionInformation("User Get By Id", "User")]
        UserGetById,
        [PermissionInformation("User download document", "User")]
        UserDocumentDownload,
        [PermissionInformation("User upload document", "User")]
        UserDocumentUpload,
        [PermissionInformation("User delete document", "User")]
        UserDocumentDelete,
        [PermissionInformation("User upload profile image", "User")]
        UserProfileImageUpload,
        #endregion

        #region Program
        [PermissionInformation("Program get list", "Program")]
        ProgramGetList,
        [PermissionInformation("Program get by id", "Program")]
        ProgramGetById,
        [PermissionInformation("Program add", "Program")]
        ProgramAdd,
        [PermissionInformation("Program update", "Program")]
        ProgramUpdate,
        [PermissionInformation("Program delete", "Program")]
        ProgramDelete,
        [PermissionInformation("Program import from excel", "Program")]
        ProgramImportFromExcel,
        [PermissionInformation("Export program excel list", "Program")]
        ProgramExportExcelList,
        //[PermissionInformation("Program get list past officer", "Program")]
        //ProgramGetListPastOfficer,
        [PermissionInformation("Program change officer", "Program")]
        ProgramChangeOfficer,
        [PermissionInformation("Program get list officer", "Program")]
        ProgramGetListOfficer,
        #endregion

        #region Student
        [PermissionInformation("Student get list", "Student")]
        StudentGetList,
        [PermissionInformation("Student get by id", "Student")]
        StudentGetById,
        [PermissionInformation("Student add", "Student")]
        StudentAdd,
        [PermissionInformation("Student update", "Student")]
        StudentUpdate,
        [PermissionInformation("Student delete", "Student")]
        StudentDelete,
        [PermissionInformation("Student import from excel", "Student")]
        StudentImportFromExcel,
        [PermissionInformation("Student download document", "Student")]
        StudentDocumentDownload,
        [PermissionInformation("Student upload document", "Student")]
        StudentDocumentUpload,
        [PermissionInformation("Student delete document", "Student")]
        StudentDocumentDelete,
        [PermissionInformation("Student excel export", "Student")]
        StudentExcelExport,
        #endregion

        #region AuthorizationCategory
        [PermissionInformation("AuthorizationCategory get list", "AuthorizationCategory")]
        AuthorizationCategoryGetList,
        [PermissionInformation("AuthorizationCategory get by id", "AuthorizationCategory")]
        AuthorizationCategoryGetById,
        [PermissionInformation("AuthorizationCategory add", "AuthorizationCategory")]
        AuthorizationCategoryAdd,
        [PermissionInformation("AuthorizationCategory update", "AuthorizationCategory")]
        AuthorizationCategoryUpdate,
        [PermissionInformation("AuthorizationCategory delete", "AuthorizationCategory")]
        AuthorizationCategoryDelete,
        #endregion

        #region AuthorizationDetail
        [PermissionInformation("AuthorizationDetail get list by program", "AuthorizationDetail")]
        AuthorizationDetailGetListByProgram,
        [PermissionInformation("AuthorizationDetail get by id", "AuthorizationDetail")]
        AuthorizationDetailGetById,
        [PermissionInformation("AuthorizationDetail add", "AuthorizationDetail")]
        AuthorizationDetailAdd,
        [PermissionInformation("AuthorizationDetail update", "AuthorizationDetail")]
        AuthorizationDetailUpdate,
        [PermissionInformation("AuthorizationDetail delete", "AuthorizationDetail")]
        AuthorizationDetailDelete,

        #endregion

        #region DependentProgram
        //[PermissionInformation("DependentProgram delete", "DependentProgram")]
        //DependentProgramDelete,
        #endregion

        #region Document

        [PermissionInformation("Document get list", "Document")]
        DocumentGetList,
        [PermissionInformation("Document get by type and id", "Document")]
        DocumentGetByType,
        [PermissionInformation("Document get by id", "Document")]
        DocumentGetById,
        [PermissionInformation("Document add", "Document")]
        DocumentAdd,
        [PermissionInformation("Document update", "Document")]
        DocumentUpdate,
        [PermissionInformation("Document delete", "Document")]
        DocumentDelete,
        [PermissionInformation("Document get deleted list", "Document")]
        DocumentGetDeletedList,
        #endregion

        #region Educator
        [PermissionInformation("Educator get list", "Educator")]
        EducatorGetList,
        [PermissionInformation("Educator get by id", "Educator")]
        EducatorGetById,
        [PermissionInformation("Educator add", "Educator")]
        EducatorAdd,
        [PermissionInformation("Educator update", "Educator")]
        EducatorUpdate,
        [PermissionInformation("Educator delete", "Educator")]
        EducatorDelete,
        [PermissionInformation("Educator download document", "Educator")]
        EducatorDocumentDownload,
        [PermissionInformation("Educator upload document", "Educator")]
        EducatorDocumentUpload,
        [PermissionInformation("Educator delete document", "Educator")]
        EducatorDocumentDelete,
        [PermissionInformation("Educator excel export", "Educator")]
        EducatorExcelExport,
        #endregion

        #region ExpertiseBranch
        [PermissionInformation("ExpertiseBranch get list", "ExpertiseBranch")]
        ExpertiseBranchGetList,
        [PermissionInformation("ExpertiseBranch get by id", "ExpertiseBranch")]
        ExpertiseBranchGetById,
        [PermissionInformation("ExpertiseBranch add", "ExpertiseBranch")]
        ExpertiseBranchAdd,
        [PermissionInformation("ExpertiseBranch update", "ExpertiseBranch")]
        ExpertiseBranchUpdate,
        [PermissionInformation("ExpertiseBranch delete", "ExpertiseBranch")]
        ExpertiseBranchDelete,
        [PermissionInformation("ExpertiseBranch upload from excel", "ExpertiseBranch")]
        ExpertiseBranchUploadFromExcel,
        #endregion

        #region Profession
        [PermissionInformation("Profession get list", "Profession")]
        ProfessionGetList,
        [PermissionInformation("Profession get by id", "Profession")]
        ProfessionGetById,
        [PermissionInformation("Profession add", "Profession")]
        ProfessionAdd,
        [PermissionInformation("Profession update", "Profession")]
        ProfessionUpdate,
        [PermissionInformation("Profession delete", "Profession")]
        ProfessionDelete,
        #endregion

        #region Faculty

        #endregion

        #region Hospital
        [PermissionInformation("Hospital get by id", "Hospital")]
        HospitalGetById,
        [PermissionInformation("Hospital add", "Hospital")]
        HospitalAdd,
        [PermissionInformation("Hospital update", "Hospital")]
        HospitalUpdate,
        [PermissionInformation("Hospital delete", "Hospital")]
        HospitalDelete,
        [PermissionInformation("Hospital excel export", "Hospital")]
        HospitalExcelExport,
        #endregion

        #region Institution
        //[PermissionInformation("Institution get list", "Institution")]
        //InstitutionGetList,
        [PermissionInformation("Institution get by id", "Institution")]
        InstitutionGetById,
        [PermissionInformation("Institution add", "Institution")]
        InstitutionAdd,
        [PermissionInformation("Institution update", "Institution")]
        InstitutionUpdate,
        [PermissionInformation("Institution delete", "Institution")]
        InstitutionDelete,
        #endregion

        #region Province
        [PermissionInformation("Province get by id", "Province")]
        ProvinceGetById,
        [PermissionInformation("Province add", "Province")]
        ProvinceAdd,
        [PermissionInformation("Province update", "Province")]
        ProvinceUpdate,
        [PermissionInformation("Province delete", "Province")]
        ProvinceDelete,
        #endregion

        #region Dashboard
        [PermissionInformation("Dashboard get map", "Dashboard")]
        DashboardGetMap,
        #endregion

        #region University
        [PermissionInformation("University get by id", "University")]
        UniversityGetById,
        [PermissionInformation("University add", "University")]
        UniversityAdd,
        [PermissionInformation("University update", "University")]
        UniversityUpdate,
        [PermissionInformation("University delete", "University")]
        UniversityDelete,
        [PermissionInformation("University excel export", "University")]
        UniversityExcelExport,
        #endregion

        #region ProtocolProgram
        [PermissionInformation("ProtocolProgram get list", "ProtocolProgram")]
        ProtocolProgramGetList,
        [PermissionInformation("ProtocolProgram get by id", "ProtocolProgram")]
        ProtocolProgramGetById,
        [PermissionInformation("ProtocolProgram add", "ProtocolProgram")]
        ProtocolProgramAdd,
        [PermissionInformation("ProtocolProgram update", "ProtocolProgram")]
        ProtocolProgramUpdate,
        [PermissionInformation("ProtocolProgram delete", "ProtocolProgram")]
        ProtocolProgramDelete,
        [PermissionInformation("ProtocolProgram download document", "ProtocolProgram")]
        ProtocolProgramDocumentDownload,
        [PermissionInformation("ProtocolProgram upload document", "ProtocolProgram")]
        ProtocolProgramDocumentUpload,
        [PermissionInformation("ProtocolProgram delete document", "ProtocolProgram")]
        ProtocolProgramDocumentDelete,
        #endregion

        //#region ComplementProgram
        //[PermissionInformation("ComplementProgram get list", "ComplementProgram")]
        //ComplementProgramGetList,
        //[PermissionInformation("ComplementProgram get paginate list", "ComplementProgram")]
        //ComplementProgramGetListPagination,
        //[PermissionInformation("ComplementProgram get by id", "ComplementProgram")]
        //ComplementProgramGetById,
        //[PermissionInformation("ComplementProgram add", "ComplementProgram")]
        //ComplementProgramAdd,
        //[PermissionInformation("ComplementProgram update", "ComplementProgram")]
        //ComplementProgramUpdate,
        //[PermissionInformation("ComplementProgram delete", "ComplementProgram")]
        //ComplementProgramDelete,
        //#endregion

        #region Affiliation
        [PermissionInformation("Affiliation get list", "Affiliation")]
        AffiliationGetList,
        [PermissionInformation("Affiliation get by id", "Affiliation")]
        AffiliationGetById,
        [PermissionInformation("Affiliation add", "Affiliation")]
        AffiliationAdd,
        [PermissionInformation("Affiliation update", "Affiliation")]
        AffiliationUpdate,
        [PermissionInformation("Affiliation delete", "Affiliation")]
        AffiliationDelete,
        [PermissionInformation("Affiliation excel export", "Affiliation")]
        AffiliationExcelExport,
        [PermissionInformation("Affiliation download document", "Affiliation")]
        AffiliationDocumentDownload,
        [PermissionInformation("Affiliation upload document", "Affiliation")]
        AffiliationDocumentUpload,
        [PermissionInformation("Affiliation delete document", "Affiliation")]
        AffiliationDocumentDelete,
        #endregion

        #region Perfection
        [PermissionInformation("Perfection get list", "Perfection")]
        PerfectionGetList,
        [PermissionInformation("Perfection get list pagination", "Perfection")]
        PerfectionGetListPagination,
        [PermissionInformation("Perfection get by id", "Perfection")]
        PerfectionGetById,
        [PermissionInformation("Perfection add", "Perfection")]
        PerfectionAdd,
        [PermissionInformation("Perfection update", "Perfection")]
        PerfectionUpdate,
        [PermissionInformation("Perfection delete", "Perfection")]
        PerfectionDelete,
        [PermissionInformation("Perfection export excel list", "Perfection")]
        PerfectionExportExcelList,
        #endregion

        #region Property
        [PermissionInformation("Property get by type", "Property")]
        PropertyGetByType,
        [PermissionInformation("Property get by id", "Property")]
        PropertyGetById,
        [PermissionInformation("Property add", "Property")]
        PropertyAdd,
        [PermissionInformation("Property update", "Property")]
        PropertyUpdate,
        [PermissionInformation("Property delete", "Property")]
        PropertyDelete,
        [PermissionInformation("Property get list", "Property")]
        PropertyGetList,
        #endregion

        #region Curriculum

        [PermissionInformation("Curriculum get list", "Curriculum")]
        CurriculumGetList,
        [PermissionInformation("Curriculum get by id", "Curriculum")]
        CurriculumGetById,
        [PermissionInformation("Curriculum add", "Curriculum")]
        CurriculumAdd,
        [PermissionInformation("Curriculum update", "Curriculum")]
        CurriculumUpdate,
        [PermissionInformation("Curriculum delete", "Curriculum")]
        CurriculumDelete,
        #endregion

        #region Title

        [PermissionInformation("Title get by id", "Title")]
        TitleGetById,
        [PermissionInformation("Title add", "Title")]
        TitleAdd,
        [PermissionInformation("Title update", "Title")]
        TitleUpdate,
        [PermissionInformation("Title delete", "Title")]
        TitleDelete,
        #endregion

        #region Decision

        [PermissionInformation("Decision get list", "Decision")]
        DecisionGetList,
        [PermissionInformation("Decision get by id", "Decision")]
        DecisionGetById,
        [PermissionInformation("Decision add", "Decision")]
        DecisionAdd,
        [PermissionInformation("Decision update", "Decision")]
        DecisionUpdate,
        [PermissionInformation("Decision delete", "Decision")]
        DecisionDelete,
        #endregion

        #region MinRequirement

        [PermissionInformation("MinRequirement get list", "MinRequirement")]
        MinRequirementGetList,
        [PermissionInformation("MinRequirement get by id", "MinRequirement")]
        MinRequirementGetById,
        [PermissionInformation("MinRequirement add", "MinRequirement")]
        MinRequirementAdd,
        [PermissionInformation("MinRequirement update", "MinRequirement")]
        MinRequirementUpdate,
        [PermissionInformation("MinRequirement delete", "MinRequirement")]
        MinRequirementDelete,
        #endregion

        #region MinRequirementForm

        [PermissionInformation("MinRequirementForm get list", "MinRequirementForm")]
        MinRequirementFormGetList,
        [PermissionInformation("MinRequirementForm get by id", "MinRequirementForm")]
        MinRequirementFormGetById,
        [PermissionInformation("MinRequirementForm add", "MinRequirementForm")]
        MinRequirementFormAdd,
        [PermissionInformation("MinRequirementForm update", "MinRequirementForm")]
        MinRequirementFormUpdate,
        [PermissionInformation("MinRequirementForm delete", "MinRequirementForm")]
        MinRequirementFormDelete,
        #endregion

        #region Standard

        //[PermissionInformation("Standard get list", "Standard")]
        //StandardGetList,
        [PermissionInformation("Standard get by id", "Standard")]
        StandardGetById,
        [PermissionInformation("Standard add", "Standard")]
        StandardAdd,
        [PermissionInformation("Standard update", "Standard")]
        StandardUpdate,
        [PermissionInformation("Standard delete", "Standard")]
        StandardDelete,
        #endregion

        #region FormStandard
        [PermissionInformation("FormStandard get list", "FormStandard")]
        FormStandardGetList,
        [PermissionInformation("FormStandard get by id", "FormStandard")]
        FormStandardGetById,
        [PermissionInformation("FormStandard add", "FormStandard")]
        FormStandardAdd,
        [PermissionInformation("FormStandard update", "FormStandard")]
        FormStandardUpdate,
        [PermissionInformation("FormStandard delete", "StandFormStandardard")]
        FormStandardDelete,
        #endregion

        #region StandardCategory

        //[PermissionInformation("StandardCategory get list", "StandardCategory")]
        //StandardCategoryGetList,
        [PermissionInformation("StandardCategory get by id", "StandardCategory")]
        StandardCategoryGetById,
        [PermissionInformation("StandardCategory add", "StandardCategory")]
        StandardCategoryAdd,
        [PermissionInformation("StandardCategory update", "StandardCategory")]
        StandardCategoryUpdate,
        [PermissionInformation("StandardCategory delete", "StandardCategory")]
        StandardCategoryDelete,
        #endregion

        #region Form

        [PermissionInformation("Form get list", "Form")]
        FormGetList,
        [PermissionInformation("Form get by id", "Form")]
        FormGetById,
        [PermissionInformation("Form add", "Form")]
        FormAdd,
        [PermissionInformation("Form update", "Form")]
        FormUpdate,
        [PermissionInformation("Form delete", "Form")]
        FormDelete,
        [PermissionInformation("Form download document", "Form")]
        FormDocumentDownload,
        [PermissionInformation("Form upload document", "Form")]
        FormDocumentUpload,
        [PermissionInformation("Form delete document", "Form")]
        FormDocumentDelete,
        #endregion

        #region Rotation

        [PermissionInformation("Rotation get list pagination", "Rotation")]
        RotationGetListPagination,
        [PermissionInformation("Rotation get by id", "Rotation")]
        RotationGetById,
        [PermissionInformation("Rotation add", "Rotation")]
        RotationAdd,
        [PermissionInformation("Rotation update", "Rotation")]
        RotationUpdate,
        [PermissionInformation("Rotation delete", "Rotation")]
        RotationDelete,
        #endregion

        #region StudentRotation

        [PermissionInformation("StudentRotation get list by student id", "StudentRotation")]
        StudentRotationGetListByStudentId,
        //[PermissionInformation("StudentRotation get former list by student id", "StudentRotation")]
        //StudentRotationGetFormerStudentListByStudentId,
        [PermissionInformation("StudentRotation get by id", "StudentRotation")]
        StudentRotationGetById,
        [PermissionInformation("StudentRotation add", "StudentRotation")]
        StudentRotationAdd,
        [PermissionInformation("StudentRotation update", "StudentRotation")]
        StudentRotationUpdate,
        [PermissionInformation("StudentRotation delete", "StudentRotation")]
        StudentRotationDelete,
		[PermissionInformation("StudentRotation delete past", "StudentRotation")]
		StudentRotationDeletePast,
		[PermissionInformation("StudentRotation download document", "StudentRotation")]
        StudentRotationDocumentDownload,
        [PermissionInformation("StudentRotation upload document", "StudentRotation")]
        StudentRotationDocumentUpload,
        [PermissionInformation("StudentRotation delete document", "StudentRotation")]
        StudentRotationDocumentDelete,
        #endregion

        #region StudentPerfection

        [PermissionInformation("StudentPerfection get list by  student id", "StudentPerfection")]
        StudentPerfectionGetListByStudentId,
        [PermissionInformation("StudentPerfection get list by  curriculum id", "StudentPerfection")]
        StudentPerfectionGetListByCurriculumId,
        [PermissionInformation("StudentPerfection get by id", "StudentPerfection")]
        StudentPerfectionGetById,
        [PermissionInformation("StudentPerfection add", "StudentPerfection")]
        StudentPerfectionAdd,
        [PermissionInformation("StudentPerfection complete all", "StudentPerfection")]
        StudentPerfectioniCompleteAll,
        [PermissionInformation("StudentPerfection update", "StudentPerfection")]
        StudentPerfectionUpdate,
        [PermissionInformation("StudentPerfection delete", "StudentPerfection")]
        StudentPerfectionDelete,
        #endregion

        #region S3

        [PermissionInformation("S3 get file by id", "S3")]
        S3GetFileById,
        [PermissionInformation("S3 get file url by id", "S3")]
        S3GetFileUrlById,
        [PermissionInformation("S3 get bucket list", "S3")]
        S3GetBucketList,
        [PermissionInformation("S3 upload file", "S3")]
        S3UploadFile,
        [PermissionInformation("S3 new upload file", "S3")]
        S3NewUploadFile,
        [PermissionInformation("S3 send file path", "S3")]
        S3SendFilePath,
        #endregion

        #region Thesis
        [PermissionInformation("Thesis get paginate list", "Thesis")]
        ThesisGetPaginateList,
        [PermissionInformation("Thesis get by id", "Thesis")]
        ThesisGetById,
        [PermissionInformation("Thesis get by student id", "Thesis")]
        ThesisGetByStudentId,
        [PermissionInformation("Thesis get list by student id for e report", "Thesis")]
        ThesisAdd,
        [PermissionInformation("Thesis update", "Thesis")]
        ThesisUpdate,
        [PermissionInformation("Thesis delete", "Thesis")]
        ThesisDelete,
        [PermissionInformation("Thesis download document", "Thesis")]
        ThesisDocumentDownload,
        [PermissionInformation("Thesis upload document", "Thesis")]
        ThesisDocumentUpload,
        [PermissionInformation("Thesis delete document", "Thesis")]
        ThesisDocumentDelete,
        #endregion

        #region PerformanceRating

        [PermissionInformation("PerformanceRating get list", "PerformanceRating")]
        PerformanceRatingGetList,
        [PermissionInformation("PerformanceRating add", "PerformanceRating")]
        PerformanceRatingAdd,
        [PermissionInformation("PerformanceRating update", "PerformanceRating")]
        PerformanceRatingUpdate,
        [PermissionInformation("PerformanceRating delete", "PerformanceRating")]
        PerformanceRatingDelete,
        [PermissionInformation("PerformanceRating download document", "PerformanceRating")]
        PerformanceRatingDocumentDownload,
        [PermissionInformation("PerformanceRating upload document", "PerformanceRating")]
        PerformanceRatingDocumentUpload,
        [PermissionInformation("PerformanceRating delete document", "PerformanceRating")]
        PerformanceRatingDocumentDelete,
        #endregion

        #region Svg

        [PermissionInformation("Svg get by Id", "Svg")]
        SvgGetById,
        #endregion

        #region Jury

        [PermissionInformation("Jury add", "Jury")]
        JuryAdd,
        [PermissionInformation("Jury delete", "Jury")]
        JuryDelete,
        #endregion

        #region OpinionForm

        [PermissionInformation("OpinionForm get list", "OpinionForm")]
        OpinionFormGetList,
        [PermissionInformation("OpinionForm get by id", "OpinionForm")]
        OpinionFormGetById,
        [PermissionInformation("OpinionForm add", "OpinionForm")]
        OpinionFormAdd,
        [PermissionInformation("OpinionForm new add", "OpinionForm")]
        OpinionFormNewAdd,
        [PermissionInformation("OpinionForm update", "OpinionForm")]
        OpinionFormUpdate,
        [PermissionInformation("OpinionForm delete", "OpinionForm")]
        OpinionFormDelete,
        [PermissionInformation("OpinionForm download document", "OpinionForm")]
        OpinionFormDocumentDownload,
        [PermissionInformation("OpinionForm upload document", "OpinionForm")]
        OpinionFormDocumentUpload,
        [PermissionInformation("OpinionForm delete document", "OpinionForm")]
        OpinionFormDocumentDelete,
        [PermissionInformation("OpinionForm example form download", "OpinionForm")]
        OpinionFormExampleDownload,
        #endregion

        #region Reason

        [PermissionInformation("Reason get by id", "Reason")]
        ReasonGetById,
        [PermissionInformation("Reason get by process type id", "Reason")]
        ReasonGetByProcessTypeId,
        [PermissionInformation("Reason add", "Reason")]
        ReasonAdd,
        [PermissionInformation("Reason update", "Reason")]
        ReasonUpdate,
        [PermissionInformation("Reason delete", "Reason")]
        ReasonDelete,
        [PermissionInformation("Reason get list pagination", "Reason")]
        ReasonGetListPagination,
        #endregion

        #region EducationTracking
        [PermissionInformation("EducationTracking get paginate list", "EducationTracking")]
        EducationTrackingGetPaginateList,
        [PermissionInformation("EducationTracking add", "EducationTracking")]
        EducationTrackingAdd,
        [PermissionInformation("EducationTracking update", "EducationTracking")]
        EducationTrackingUpdate,
        [PermissionInformation("EducationTracking delete", "EducationTracking")]
        EducationTrackingDelete,
        [PermissionInformation("EducationTracking download document", "EducationTracking")]
        EducationTrackingDocumentDownload,
        [PermissionInformation("EducationTracking upload document", "EducationTracking")]
        EducationTrackingDocumentUpload,
        [PermissionInformation("EducationTracking delete document", "EducationTracking")]
        EducationTrackingDocumentDelete,
        [PermissionInformation("EducationTracking estimated finish update", "EducationTracking")]
        EducationTrackingEstimatedFinishUpdate,
        #endregion

        #region ProgressReport
        [PermissionInformation("ProgressReport add", "ProgressReport")]
        ProgressReportAdd,
        [PermissionInformation("ProgressReport update", "ProgressReport")]
        ProgressReportUpdate,
        [PermissionInformation("ProgressReport delete", "ProgressReport")]
        ProgressReportDelete,
        [PermissionInformation("ProgressReport download document", "ProgressReport")]
        ProgressReportDocumentDownload,
        [PermissionInformation("ProgressReport upload document", "ProgressReport")]
        ProgressReportDocumentUpload,
        [PermissionInformation("ProgressReport delete document", "ProgressReport")]
        ProgressReportDocumentDelete,
        #endregion

        #region AdvisorThesis
        [PermissionInformation("AdvisorThesis get by id", "AdvisorThesis")]
        AdvisorThesisGetById,
        [PermissionInformation("AdvisorThesis add", "AdvisorThesis")]
        AdvisorThesisAdd,
        [PermissionInformation("AdvisorThesis delete", "AdvisorThesis")]
        AdvisorThesisDelete,
        [PermissionInformation("AdvisorThesis get list", "AdvisorThesis")]
        AdvisorThesisGetList,
        #endregion

        #region Notification
        [PermissionInformation("Notification get by id", "Notification")]
        NotificationGetById,
        [PermissionInformation("Notification add", "Notification")]
        NotificationAdd,
        [PermissionInformation("Notification delete", "Notification")]
        NotificationDelete,
        [PermissionInformation("Notification get list by user id", "Notification")]
        NotificationGetListByUserId,
        [PermissionInformation("Notification get paginate list", "Notification")]
        NotificationGetPaginateList,
        #endregion

        #region EthicCommitteeDecision
        [PermissionInformation("EthicCommitteeDecision add", "EthicCommitteeDecision")]
        EthicCommitteeDecisionAdd,
        [PermissionInformation("EthicCommitteeDecision update", "EthicCommitteeDecision")]
        EthicCommitteeDecisionUpdate,
        [PermissionInformation("EthicCommitteeDecision delete", "EthicCommitteeDecision")]
        EthicCommitteeDecisionDelete,
        [PermissionInformation("EthicCommitteeDecision get paginate list", "EthicCommitteeDecision")]
        EthicCommitteeDecisionGetPaginateList,
        [PermissionInformation("EthicCommitteeDecision download document", "EthicCommitteeDecision")]
        EthicCommitteeDecisionDocumentDownload,
        [PermissionInformation("EthicCommitteeDecision upload document", "EthicCommitteeDecision")]
        EthicCommitteeDecisionDocumentUpload,
        [PermissionInformation("EthicCommitteeDecision delete document", "EthicCommitteeDecision")]
        EthicCommitteeDecisionDocumentDelete,
        #endregion

        #region OfficialLetter
        [PermissionInformation("OfficialLetter get by id", "OfficialLetter")]
        OfficialLetterGetById,
        [PermissionInformation("OfficialLetter add", "OfficialLetter")]
        OfficialLetterAdd,
        [PermissionInformation("OfficialLetter update", "OfficialLetter")]
        OfficialLetterUpdate,
        [PermissionInformation("OfficialLetter delete", "OfficialLetter")]
        OfficialLetterDelete,
        [PermissionInformation("OfficialLetter get paginate list", "OfficialLetter")]
        OfficialLetterGetPaginateList,
        [PermissionInformation("OfficialLetter get list by thesis id", "OfficialLetter")]
        OfficialLetterGetListByThesisId,
        [PermissionInformation("OfficialLetter download document", "OfficialLetter")]
        OfficialLetterDocumentDownload,
        [PermissionInformation("OfficialLetter upload document", "OfficialLetter")]
        OfficialLetterDocumentUpload,
        [PermissionInformation("OfficialLetter delete document", "OfficialLetter")]
        OfficialLetterDocumentDelete,
        #endregion

        #region ThesisDefence
        [PermissionInformation("ThesisDefence get by id", "ThesisDefence")]
        ThesisDefenceGetById,
        [PermissionInformation("ThesisDefence add", "ThesisDefence")]
        ThesisDefenceAdd,
        [PermissionInformation("ThesisDefence update", "ThesisDefence")]
        ThesisDefenceUpdate,
        [PermissionInformation("ThesisDefence delete", "ThesisDefence")]
        ThesisDefenceDelete,
        [PermissionInformation("ThesisDefence get list by thesis id", "ThesisDefence")]
        ThesisDefenceGetListByThesisId,
        [PermissionInformation("ThesisDefence download document", "ThesisDefence")]
        ThesisDefenceDocumentDownload,
        [PermissionInformation("ThesisDefence upload document", "ThesisDefence")]
        ThesisDefenceDocumentUpload,
        [PermissionInformation("ThesisDefence delete document", "ThesisDefence")]
        ThesisDefenceDocumentDelete,
        #endregion

        #region CurriculumRotation

        //[PermissionInformation("Curriculum Rotation get list by  Curriculum id", "CurriculumRotation")]
        //CurriculumRotationGetListByCurriculumId,
        //[PermissionInformation("Curriculum Rotation get paginate list", "CurriculumRotation")]
        //CurriculumRotationGetPaginateList,
        //[PermissionInformation("Curriculum Rotation get by id", "CurriculumRotation")]
        //CurriculumRotationGetById,
        //[PermissionInformation("Curriculum Rotation add", "CurriculumRotation")]
        //CurriculumRotationAdd,
        //[PermissionInformation("Curriculum Rotation put", "CurriculumRotation")]
        //CurriculumRotationPut,
        //[PermissionInformation("Curriculum Rotation delete", "CurriculumRotation")]
        //CurriculumRotationDelete,
        #endregion

        #region CurriculumPerfection

        //[PermissionInformation("Curriculum Perfection get paginate list", "CurriculumPerfection")]
        //CurriculumPerfectionGetPaginateList,
        //[PermissionInformation("Curriculum Perfection get by id", "CurriculumPerfection")]
        //CurriculumPerfectionGetById,
        //[PermissionInformation("Curriculum Perfection add", "CurriculumPerfection")]
        //CurriculumPerfectionAdd,
        //[PermissionInformation("Curriculum Perfection put", "CurriculumPerfection")]
        //CurriculumPerfectionPut,
        //[PermissionInformation("Curriculum Perfection delete", "CurriculumPerfection")]
        //CurriculumPerfectionDelete,
        #endregion

        #region Country
        [PermissionInformation("Country get list", "Country")]
        CountryGetList,
        [PermissionInformation("Country get by id", "Country")]
        CountryGetById,
        [PermissionInformation("Country add", "Country")]
        CountryAdd,
        [PermissionInformation("Country update", "Country")]
        CountryUpdate,
        [PermissionInformation("Country delete", "Country")]
        CountryDelete,
        [PermissionInformation("Country get list pagination", "Country")]
        CountryGetListPagination,
        #endregion

        #region Demand
        [PermissionInformation("Demand get list", "Demand")]
        DemandGetList,
        [PermissionInformation("Demand get by id", "Demand")]
        DemandGetById,
        [PermissionInformation("Demand add", "Demand")]
        DemandAdd,
        [PermissionInformation("Demand update", "Demand")]
        DemandUpdate,
        [PermissionInformation("Demand delete", "Demand")]
        DemandDelete,
        [PermissionInformation("Demand get list pagination", "Demand")]
        DemandGetListPagination,
        #endregion

        #region Announcement        
        [PermissionInformation("Announcement get by id", "Announcement")]
        AnnouncementGetById,
        [PermissionInformation("Announcement add", "Announcement")]
        AnnouncementAdd,
        [PermissionInformation("Announcement update", "Announcement")]
        AnnouncementUpdate,
        [PermissionInformation("Announcement delete", "Announcement")]
        AnnouncementDelete,
        [PermissionInformation("Announcement get list pagination", "Announcement")]
        AnnouncementGetListPagination,
        #endregion

        #region Study
        [PermissionInformation("Study get list", "Study")]
        StudyGetList,
        [PermissionInformation("Study get by id", "Study")]
        StudyGetById,
        [PermissionInformation("Study add", "Study")]
        StudyAdd,
        [PermissionInformation("Study update", "Study")]
        StudyUpdate,
        [PermissionInformation("Study delete", "Study")]
        StudyDelete,
        #endregion

        #region ScientificStudy
        [PermissionInformation("ScientificStudy get list", "ScientificStudy")]
        ScientificStudyGetList,
        [PermissionInformation("ScientificStudy get by id", "ScientificStudy")]
        ScientificStudyGetById,
        [PermissionInformation("ScientificStudy add", "ScientificStudy")]
        ScientificStudyAdd,
        [PermissionInformation("ScientificStudy update", "ScientificStudy")]
        ScientificStudyUpdate,
        [PermissionInformation("ScientificStudy delete", "ScientificStudy")]
        ScientificStudyDelete,
        [PermissionInformation("ScientificStudy download document", "ScientificStudy")]
        ScientificStudyDocumentDownload,
        [PermissionInformation("ScientificStudy upload document", "ScientificStudy")]
        ScientificStudyDocumentUpload,
        [PermissionInformation("ScientificStudy delete document", "ScientificStudy")]
        ScientificStudyDocumentDelete,
        #endregion

        #region ExitExam
        [PermissionInformation("ExitExam get paginate list", "ExitExam")]
        ExitExamGetListPagination,
        [PermissionInformation("ExitExam get list", "ExitExam")]
        ExitExamGetList,
        [PermissionInformation("ExitExam get by id", "ExitExam")]
        ExitExamGetById,
        [PermissionInformation("ExitExam add", "ExitExam")]
        ExitExamAdd,
        [PermissionInformation("ExitExam update", "ExitExam")]
        ExitExamUpdate,
        [PermissionInformation("ExitExam delete", "ExitExam")]
        ExitExamDelete,
        #endregion

        #region Log
        [PermissionInformation("Log get by id", "Log")]
        LogGetById,
        [PermissionInformation("Log get list pagination", "Log")]
        LogGetListPagination,
        #endregion

        #region UserSetting
        [PermissionInformation("UserSetting get by user", "UserSetting")]
        UserSettingGetByUser,
        [PermissionInformation("UserSetting change role", "UserSetting")]
        UserSettingChangeRole,
        #endregion

        #region Menu
        [PermissionInformation("Menu get list", "Menu")]
        MenuGetList,
        [PermissionInformation("Menu get by id", "Menu")]
        MenuGetById,
        [PermissionInformation("Menu add", "Menu")]
        MenuAdd,
        [PermissionInformation("Menu update", "Menu")]
        MenuUpdate,
        [PermissionInformation("Menu delete", "Menu")]
        MenuDelete,
        [PermissionInformation("Menu get list pagination", "Menu")]
        MenuGetListPagination,
        #endregion

        #region Archive
        [PermissionInformation("Archive get university list", "Archive")]
        ArchiveGetUniversityList,
        [PermissionInformation("Archive get hospital list", "Archive")]
        ArchiveGetHospitalList,
        [PermissionInformation("Archive get program list", "Archive")]
        ArchiveGetProgramList,
        [PermissionInformation("Archive get protocol program list", "Archive")]
        ArchiveGetProtocolProgramList,
        [PermissionInformation("Archive get curriculum list", "Archive")]
        ArchiveGetCurriculumList,
        [PermissionInformation("Archive get student list", "Archive")]
        ArchiveGetStudentList,
        [PermissionInformation("Archive get educator list", "Archive")]
        ArchiveGetEducatorList,
        [PermissionInformation("Archive get user list", "Archive")]
        ArchiveGetUserList,
        [PermissionInformation("Archive get document list", "Archive")]
        ArchiveGetDocumentList,

        [PermissionInformation("Archive undelete educator", "Archive")]
        ArchiveUndeleteEducator,
        [PermissionInformation("Archive undelete student", "Archive")]
        ArchiveUndeleteStudent,
        [PermissionInformation("Archive undelete university", "Archive")]
        ArchiveUndeleteUniversity,
        [PermissionInformation("Archive undelete hospital", "Archive")]
        ArchiveUndeleteHospital,
        [PermissionInformation("Archive undelete program", "Archive")]
        ArchiveUndeleteProgram,
        [PermissionInformation("Archive undelete protocol program", "Archive")]
        ArchiveUndeleteProtocolProgram,
        [PermissionInformation("Archive undelete curriculum", "Archive")]
        ArchiveUndeleteCurriculum,
        [PermissionInformation("Archive undelete curriculum perfection", "Archive")]
        ArchiveUndeleteCurriculumPerfection,
        [PermissionInformation("Archive undelete curriculum rotation", "Archive")]
        ArchiveUndeleteCurriculumRotation,
        [PermissionInformation("Archive undelete user", "Archive")]
        ArchiveUndeleteUser,
        [PermissionInformation("Archive undelete document", "Archive")]
        ArchiveUndeleteDocument,
        #endregion

        #region EReport
        [PermissionInformation("EReport get", "EReport")]
        EReportGet,
        #endregion

        #region SpecificEducation

        [PermissionInformation("SpecificEducation get list", "SpecificEducation")]
        SpecificEducationGetList,
        [PermissionInformation("SpecificEducation get paginate list", "SpecificEducation")]
        SpecificEducationPaginateList,
        [PermissionInformation("SpecificEducation get by id", "SpecificEducation")]
        SpecificEducationGetById,
        [PermissionInformation("SpecificEducation add", "SpecificEducation")]
        SpecificEducationAdd,
        [PermissionInformation("SpecificEducation update", "SpecificEducation")]
        SpecificEducationUpdate,
        [PermissionInformation("SpecificEducation delete", "SpecificEducation")]
        SpecificEducationDelete,
        #endregion
        #region  StudentsSpecificEducation

        [PermissionInformation("StudentsSpecificEducation get list", "StudentsSpecificEducation")]
        StudentsSpecificEducationGetList,
        [PermissionInformation("StudentsSpecificEducation get paginate list", "StudentsSpecificEducation")]
        StudentsSpecificEducationPagianateList,
        [PermissionInformation("StudentsSpecificEducation get by id", "StudentsSpecificEducation")]
        StudentsSpecificEducationGetById,
        [PermissionInformation("StudentsSpecificEducation add", "StudentsSpecificEducation")]
        StudentsSpecificEducationAdd,
        [PermissionInformation("StudentsSpecificEducation update", "StudentsSpecificEducation")]
        StudentsSpecificEducationUpdate,
        [PermissionInformation("StudentsSpecificEducation delete", "StudentsSpecificEducation")]
        StudentsSpecificEducationDelete,
        #endregion

        #region SpecificEducationPlace

        //[PermissionInformation("SpecificEducationPlace get list", "SpecificEducationPlace")]
        //SpecificEducationPlaceGetList,
        //[PermissionInformation("SpecificEducationPlace get paginate list", "SpecificEducationPlace")]
        //SpecificEducationPlacePagianateList,
        [PermissionInformation("SpecificEducationPlace get by id", "SpecificEducationPlace")]
        SpecificEducationPlaceGetById,
        [PermissionInformation("SpecificEducationPlace add", "SpecificEducationPlace")]
        SpecificEducationPlaceAdd,
        [PermissionInformation("SpecificEducationPlace update", "SpecificEducationPlace")]
        SpecificEducationPlaceUpdate,
        [PermissionInformation("SpecificEducationPlace delete", "SpecificEducationPlace")]
        SpecificEducationPlaceDelete,
        #endregion

        #region QuotaRequest

        [PermissionInformation("QuotaRequest get paginate list", "QuotaRequest")]
        QuotaRequestGetPaginateList,
        [PermissionInformation("QuotaRequest get by id", "QuotaRequest")]
        QuotaRequestGetById,
        [PermissionInformation("QuotaRequest add", "QuotaRequest")]
        QuotaRequestAdd,
        [PermissionInformation("QuotaRequest update", "QuotaRequest")]
        QuotaRequestUpdate,
        [PermissionInformation("QuotaRequest delete", "QuotaRequest")]
        QuotaRequestDelete,
        #endregion

        #region SubQuotaRequest

        [PermissionInformation("SubQuotaRequest get paginate list", "SubQuotaRequest")]
        SubQuotaRequestGetPaginateList,
        [PermissionInformation("SubQuotaRequest get by id", "SubQuotaRequest")]
        SubQuotaRequestGetById,
        [PermissionInformation("SubQuotaRequest add", "SubQuotaRequest")]
        SubQuotaRequestAdd,
        [PermissionInformation("SubQuotaRequest update", "SubQuotaRequest")]
        SubQuotaRequestUpdate,
        [PermissionInformation("SubQuotaRequest delete", "SubQuotaRequest")]
        SubQuotaRequestDelete,
        #endregion

         #region EducatorCountContributionFormula

        [PermissionInformation("EducatorCountContributionFormula get paginate list", "EducatorCountContributionFormula")]
        EducatorCountContributionFormulaGetPaginateList,
        [PermissionInformation("EducatorCountContributionFormula get by id", "EducatorCountContributionFormula")]
        EducatorCountContributionFormulaGetById,
        [PermissionInformation("EducatorCountContributionFormula add", "EducatorCountContributionFormula")]
        EducatorCountContributionFormulaAdd,
        [PermissionInformation("EducatorCountContributionFormula update", "EducatorCountContributionFormula")]
        EducatorCountContributionFormulaUpdate,
        [PermissionInformation("EducatorCountContributionFormula delete", "EducatorCountContributionFormula")]
        EducatorCountContributionFormulaDelete,
        #endregion

        #region Portfolio

        [PermissionInformation("Portfolio get paginate list", "Portfolio")]
        PortfolioGetPaginateList,
        [PermissionInformation("Portfolio get by id", "Portfolio")]
        PortfolioGetById,
        [PermissionInformation("Portfolio add", "Portfolio")]
        PortfolioAdd,
        [PermissionInformation("Portfolio update", "Portfolio")]
        PortfolioUpdate,
        [PermissionInformation("Portfolio delete", "Portfolio")]
        PortfolioDelete,
        #endregion

        #region StudentCount

        [PermissionInformation("StudentCount get paginate list", "StudentCount")]
        StudentCountGetPaginateList,
        [PermissionInformation("StudentCount get by id", "StudentCount")]
        StudentCountGetById,
        [PermissionInformation("StudentCount add", "StudentCount")]
        StudentCountAdd,
        [PermissionInformation("StudentCount update", "StudentCount")]
        StudentCountUpdate,
        [PermissionInformation("StudentCount delete", "StudentCount")]
        StudentCountDelete,
        #endregion

        #region ENabiz
        [PermissionInformation("ENabiz Get Student List", "E-Nabiz")]
        ENabizGetStudentList,
        [PermissionInformation("ENabiz Get Expertise Branch List", "E-Nabiz")]
        ENabizGetExpertiseBranchList,
        #endregion

        #region OSYM
        [PermissionInformation("OSYM Get Student Information", "OSYM")]
        OSYMGetStudentInformation,
        [PermissionInformation("OSYM Post Last Exam Results", "OSYM")]
        OSYMPostLastExamResults,
        #endregion
    }
}
