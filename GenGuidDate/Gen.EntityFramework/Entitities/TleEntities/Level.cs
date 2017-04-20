using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework.Entitities.TleEntities
{
    public class Level
    {
        [Required]
        [Key]
        public System.Guid LevelID { get; set; }
        public string LevelCode { get; set; }
        public string LevelName { get; set; }
        public Nullable<int> LevelType { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.Guid> UpdateUserID { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> LevelSize { get; set; }
    }
}
