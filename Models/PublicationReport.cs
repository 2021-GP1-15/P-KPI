using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("PublicationReport")]
    public partial class PublicationReport
    {
        [Key]
        public int PublicationReportId { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }
        [Column("NumOfFaculty_oneP")]
        public int? NumOfFacultyOneP { get; set; }
        public int? NumOfPublications { get; set; }
        public int? NumOfCitations { get; set; }
        [StringLength(50)]
        public string Year { get; set; }
        public int? ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.PublicationReports))]
        public virtual UniProgram Program { get; set; }
    }
}
