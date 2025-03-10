using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
	public enum StudentDeleteReasonType
	{
		[Description("Registration by mistake")]
		RegistrationByMistake = 1,
		[Description("Conviction")]
		Conviction = 2,
		[Description("Being Considered Independent")]
		BeingConsideredIndependent = 3,
		[Description("Exportation")]
		Exportation = 4,
		[Description("Banned from Profession")]
		BannedFromProfession = 5,
		[Description("Branch Change")]
		BranchChange = 6,
		[Description("Other")]
		Other = 7,

	}
}
 