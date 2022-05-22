using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("BatchStatistic")]
    public partial class BatchStatistic
    {
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }
        public int? NumOfBatchStudent { get; set; }
        public int? NumMinYearStudent { get; set; }
        public int? NumFirstYearStudent { get; set; }
        public int? NumStudentContinue { get; set; }
        [Key]
        [Column("BatchID")]
        public int BatchId { get; set; }
        [Column("programId")]
        public int? ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.BatchStatistics))]
        public virtual UniProgram Program { get; set; }
    }
}
