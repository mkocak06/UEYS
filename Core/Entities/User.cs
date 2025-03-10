using Core.Entities.Koru;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class User : ExtendedBaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string IdentityNo { get; set; }
        public string OldIdentityNo { get; set; }
        public string Name { get; set; }
        public string OldName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public GenderType? Gender { get; set; }
        public bool? IsForeign { get; set; }
        public bool IsReadClarification { get; set; } = false;
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
        public ForeignType? ForeignType { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime? LastLoginDate { get; set; } = DateTime.UtcNow;
        public bool IsPassive { get; set; } = false;
        public long? CountryId { get; set; }
        public virtual Country Country { get; set; }
        public long? InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        public long SelectedRoleId { get; set; }
        public bool? IsPhoneVerified { get; set; }
        public bool? IsMailVerified { get; set; }
        public int? PhoneVerificationCode { get; set; }
        public int? MailVerificationCode { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<AdvisorThesis> AdvisorTheses { get; set; }
        public virtual ICollection<Educator> Educators { get; set; }
        public virtual ICollection<University> Universities { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<OpinionForm> OpinionForms { get; set; }
        public virtual ICollection<ExitExam> ExitExams { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public virtual ICollection<OnSiteVisitCommittee> OnSiteVisitCommittees { get; set; }
        public virtual ICollection<Jury> Juries{ get; set; }

    }
}
