using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class II_Instance
    {
        [Key]
        [Required]
        public string IIInstanceId { get; set; }
        public string PKName { get; set; }
        public string RecTeam { get; set; }
        public string User { get; set; }
        public string IIClassId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string IsPack { get; set; }
        public string IIInstanceStatusId { get; set; }
        public Nullable<System.DateTime> StatusChangeTime { get; set; }
        public string Attachments { get; set; }
        public string Mentor { get; set; }
        public string IsAnswered { get; set; }
        public string IsEvaluate { get; set; }
        public string IndividualCost { get; set; }
        public string IIClassRoundId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}