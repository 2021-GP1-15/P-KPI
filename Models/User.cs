using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("User")]
    public partial class User
    {
        [Key]
        [StringLength(50)]
        public string UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string Privilege { get; set; }
        [StringLength(500)]
        public string Password { get; set; }
    }
}
