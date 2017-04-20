using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Personal_Profile
    {
        [Required]
        [Key]
        public string PersonalProfileId { get; set; }
        public string RecTeam { get; set; }
        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string Name_Local_Language { get; set; }
        public string Password { get; set; }
        public string JobId { get; set; }
        public string Team { get; set; }
        public string Mailbox { get; set; }
        public string Gender { get; set; }
        public string LocationId { get; set; }
        public string Attachments { get; set; }
        public string IsActive { get; set; }
        public string EmployeeTypeId { get; set; }
        public string CompanyId { get; set; }
        public string Resume { get; set; }
        public Nullable<int> Birth { get; set; }
        public string Recive_System_Mail { get; set; }
        public string SelfIntroduction { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
        public string IsNeedApproval_LI { get; set; }
        public string Is17025 { get; set; }
        public string AllocatableCredits { get; set; }
        public string AcquiredCredits { get; set; }
    }
}