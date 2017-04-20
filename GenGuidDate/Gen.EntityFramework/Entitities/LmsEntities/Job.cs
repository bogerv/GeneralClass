using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Job
    {
        [Required]
        [Key]
        public string JobId { get; set; }
        public string JobTitle { get; set; }
        public string HRLevelId { get; set; }
        public string TeamId { get; set; }
    }
}