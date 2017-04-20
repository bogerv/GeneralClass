using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Area_Pack
    {
        [Required]
        [Key]
        public string AreaPackId { get; set; }
        public string RecTeam { get; set; }
        public string Name { get; set; }
        public string Includings { get; set; }
        public string Expert { get; set; }
        public string Des { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
        public string Logo { get; set; }
    }
}