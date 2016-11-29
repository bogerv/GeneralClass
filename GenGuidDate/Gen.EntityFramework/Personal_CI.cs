using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Personal_CI
    {
        [Key]
        [Required]
        public string PersonalCIId { get; set; }
        public string PKName { get; set; }
        public string RecTeam { get; set; }
        public string PersonalProfileId { get; set; }
        public string CITreeId { get; set; }
        public string AreaPackId { get; set; }
        public string Status { get; set; }
        public string Mentor { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}