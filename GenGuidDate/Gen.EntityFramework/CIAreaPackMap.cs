using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class CI_Area_Map
    {
        [Required]
        [Key]
        public string CIAreaMapId { get; set; }
        public string CITreeId { get; set; }
        public string AreaPackId { get; set; }
        public string RecTeam { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}