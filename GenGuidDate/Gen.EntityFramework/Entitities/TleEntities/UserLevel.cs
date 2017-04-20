using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework.Entitities.TleEntities
{
    public class UserLevel
    {
        [Required]
        [Key]
        public System.Guid UserLvlID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<int> TestCategory { get; set; }
        public Nullable<System.Guid> ModalityID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<System.Guid> LevelID { get; set; }
        public Nullable<int> LevelSize { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> LastDueTime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.Guid> UpdateUserID { get; set; }
    }
}
