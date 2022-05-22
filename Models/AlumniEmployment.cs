using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("AlumniEmployment")]
    public partial class AlumniEmployment
    {
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }
        public int? GradEmployed { get; set; }
        public int? GradEnrolled { get; set; }
        [Key]
        [Column("AlumniID")]
        public int AlumniId { get; set; }
        [Column("programId")]
        public int? ProgramId { get; set; }
        public int? NumOfStudentPassQiyasExam { get; set; }
        public int? NumOfStudentPassCareerExam { get; set; }
        public double? TotalGradEvalByCompany { get; set; }
        public int? NumOfCompanyEvaled { get; set; }

        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.AlumniEmployments))]
        public virtual UniProgram Program { get; set; }
    }
}
