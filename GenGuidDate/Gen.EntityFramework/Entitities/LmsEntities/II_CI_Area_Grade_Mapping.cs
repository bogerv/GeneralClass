using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class II_CI_Area_Grade_Mapping
    {
        [Key]
        [Required]
        public string IICIAreaGradeMappingId { get; set; }
        public string PKName { get; set; }
        public string RecTeam { get; set; }
        public string IIClassId { get; set; }
        public string CIAreaMapId { get; set; }
        public string IsIIMandatory { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}