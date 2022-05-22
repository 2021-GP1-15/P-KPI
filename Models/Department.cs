using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("Department")]
    public partial class Department
    {
        public Department()
        {
            UniPrograms = new HashSet<UniProgram>();
        }

        [Key]
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
        [StringLength(50)]
        public string DepartmentName { get; set; }
        [Column("CollegeID")]
        public int? CollegeId { get; set; }

        [ForeignKey(nameof(CollegeId))]
        [InverseProperty("Departments")]
        public virtual College College { get; set; }
        [InverseProperty(nameof(UniProgram.Department))]
        public virtual ICollection<UniProgram> UniPrograms { get; set; }
    }
}
