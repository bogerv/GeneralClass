using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework.Entitities.TleEntities
{
    public class Modality
    {
        [Required]
        [Key]
        public System.Guid ModalityID { get; set; }
        public string ModalityCode { get; set; }
        public Nullable<int> ModalityType { get; set; }
        public string ModalityName { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.Guid> UpdateUserID { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string Remark { get; set; }
        public Nullable<int> TestCategory { get; set; }
        public Nullable<int> CsStaffID { get; set; }
    }
}
