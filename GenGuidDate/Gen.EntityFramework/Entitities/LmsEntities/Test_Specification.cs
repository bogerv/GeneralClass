using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Test_Specification
    {
        [Required]
        [Key]
        public string TestSpecificationId { get; set; }
        public string StandardOrgTestSpecNo { get; set; }
        public string Name { get; set; }
        public string RecTeam { get; set; }
        public string TestSpcNo { get; set; }
        public string TestSpecType { get; set; }
        public string TeamId { get; set; }
        public string StandardOrgId { get; set; }
        public string LanguageId { get; set; }
        public string LatestVersion { get; set; }
        public Nullable<System.DateTime> StandardCheckDate { get; set; }
        public Nullable<System.DateTime> JoinIn17025Date { get; set; }
        public string ResponsiblePersonId { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string IsDeleted { get; set; }
        public string AuthorizedSignatory { get; set; }
        public string CheckedBy { get; set; }
    }
}