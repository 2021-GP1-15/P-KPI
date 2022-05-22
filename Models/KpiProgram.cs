using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("KPIProgram")]
    public partial class Kpiprogram
    {
        [Key]
        [Column("KPIProgramID")]
        public int KpiprogramId { get; set; }
        [Column("KPI_ID")]
        public int KpiId { get; set; }
        [Column("ProgramID")]
        public int ProgramId { get; set; }
        public double? TargetBenchmark { get; set; }
        public double? InternalBenchmark { get; set; }
        public double? ExternalBenchmark { get; set; }
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(50)]
        public string Term { get; set; }
        [Column("ActualKPIValue")]
        public double? ActualKpivalue { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }

        [ForeignKey(nameof(KpiId))]
        [InverseProperty("Kpiprograms")]
        public virtual Kpi Kpi { get; set; }
        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.Kpiprograms))]
        public virtual UniProgram Program { get; set; }
    }
}
