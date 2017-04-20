using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Job_ReqOnAreaPack
    {
        [Key]
        [Required]
        public string JobReqOnAreaPackId { get; set; }
        public string PKName { get; set; }
        public string RecTeam { get; set; }
        public string JobId { get; set; }
        public string CIAreaMapId { get; set; }
        public string IsIIMandatory { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}