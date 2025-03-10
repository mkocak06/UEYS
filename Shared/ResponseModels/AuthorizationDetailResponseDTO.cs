using Shared.BaseModels;
using System;

namespace Shared.ResponseModels
{
    public class AuthorizationDetailResponseDTO : AuthorizationDetailBase
    {
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual AuthorizationCategoryResponseDTO AuthorizationCategory { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
