using System;
using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class II_Class
    {
        [Required]
        [Key]
        public string IIClassId { get; set; }
        public string Name { get; set; }
        public string Mentor { get; set; }
    }
}