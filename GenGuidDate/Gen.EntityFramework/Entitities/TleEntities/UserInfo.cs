using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework.Entitities.TleEntities
{
    public class UserInfo
    {
        [Required]
        [Key]
        public System.Guid UserID { get; set; }
        public string Code1ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string Email { get; set; }
        public Nullable<int> Gender { get; set; }
        public string FSEID { get; set; }
        public string LineManagerEmail { get; set; }
        public string DistrictName { get; set; }
        public string PositionName { get; set; }
        public Nullable<int> IsEngineer { get; set; }
        public Nullable<System.DateTime> OnBoardDate { get; set; }
        public Nullable<int> UserType { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.Guid> UpdateUserID { get; set; }
        public Nullable<int> CsStaffID { get; set; }
    }
}
