using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class HRLevel
    {
        [Required]
        [Key]
        public string HRLevelId { get; set; }
        public string Name { get; set; }
    }
}