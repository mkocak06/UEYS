using System.ComponentModel;

namespace Shared.Types
{
    public enum EducatorDeleteReasonType
    {
        [Description("Retirement")]
        Retirement = 1,
        [Description("Resignation")]
        Resignation = 2,
        [Description("Death")]
        Death = 3,
        [Description("Transfer to an Institution that Does Not Provide Specialization Education")]
        TransferToAnInstitutionThatDoesNotProvideSpecializationEducation = 4,
		[Description("Exportation")]
		Exportation = 5,
		[Description("Banned from Profession")]
		BannedFromProfession = 6,
		[Description("Other")]
        Other= 7,
    }
}
