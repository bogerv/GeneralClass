using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class CI_Tree
    {
        [Required]
        [Key]
        public string CITreeId { get; set; }
        public string Name { get; set; }
        public string RecTeam { get; set; }
        public string NameinRoot { get; set; }
        public string Parent { get; set; }
        public Nullable<int> Depth { get; set; }
        public string Code { get; set; }
        public string Des { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string IsDeleted { get; set; }
    }
}