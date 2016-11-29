using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Person_Job_Map
    {
        [Required]
        [Key]
        public string PersonJobMapId { get; set; }
        public string PersonalProfileId { get; set; }
        public string JobId { get; set; }
        public string IsMain { get; set; }
        public Nullable<int> StartTime { get; set; }
        public Nullable<int> EndTime { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string PersonJobStatusId { get; set; }
        public string AuthorizedBy { get; set; }
        public Nullable<System.DateTime> AuthorizedOn { get; set; }
        public Nullable<System.DateTime> AuthorizedEndTime { get; set; }
    }
}