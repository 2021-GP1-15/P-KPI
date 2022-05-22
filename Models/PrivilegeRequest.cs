using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("privilegeRequest ")]
    public partial class PrivilegeRequest
    {
        public PrivilegeRequest(string privilegeType, string privilegeRes, string userId)
        {
            PrivilegeType = privilegeType;
            PrivilegeRes = privilegeRes;
            UserId = userId;
        }

        [Key]
        [Column("RequestID")]
        public int RequestId { get; set; }
        [StringLength(50)]
        public string PrivilegeType { get; set; }
        [StringLength(1000)]
        public string PrivilegeRes { get; set; }
        [StringLength(50)]
        public string UserId { get; set; }
    }
}
