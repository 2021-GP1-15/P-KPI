using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("ar")]
    public partial class Ar
    {
        [Key]
        [StringLength(50)]
        public string Name { get; set; }
        public int? Age { get; set; }
        [Column("grade")]
        public double? Grade { get; set; }
    }
}
