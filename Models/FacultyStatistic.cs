using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("FacultyStatistic")]
    public partial class FacultyStatistic
    {
        [Key]
        [Column("FacultyID")]
        public int FacultyId { get; set; }
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }
        [Column("ProgramID")]
        public int? ProgramId { get; set; }
        [StringLength(100)]
        public string Type { get; set; }
        public int? NumOfRespondent { get; set; }
        [Column("numOfNeutralScore")]
        public int? NumOfNeutralScore { get; set; }
        [Column("numOfWeakScore")]
        public int? NumOfWeakScore { get; set; }
        public double? SurveyScore { get; set; }
        public int? NumOfGoodScore { get; set; }
        public int? NumOfExellentScore { get; set; }
        public int? NumOfProf { get; set; }
        public int? NumOfTeacher { get; set; }
        public int? NumOfAssociateProf { get; set; }
        public int? NumOfLecturer { get; set; }
        public int? NumOfTeachingAssistant { get; set; }
        public int? NumOfResignation { get; set; }
        public int? NumOfAssistantProf { get; set; }

        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.FacultyStatistics))]
        public virtual UniProgram Program { get; set; }
    }
}
