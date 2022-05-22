using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("College")]
    public partial class College
    {
        public College()
        {
            Departments = new HashSet<Department>();
        }

        [Key]
        [Column("CollegeID")]
        public int CollegeId { get; set; }
        [StringLength(50)]
        public string CollageName { get; set; }

        [InverseProperty(nameof(Department.College))]
        public virtual ICollection<Department> Departments { get; set; }
    }
}
