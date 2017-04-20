using System.ComponentModel.DataAnnotations;

namespace Gen.EntityFramework
{
    public class Team
    {
        [Required]
        [Key]
        public string TeamId { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string NameinRoot { get; set; }
    }
}