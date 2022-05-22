using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("KPI")]
    public partial class Kpi
    {
        public Kpi()
        {
            Kpiprograms = new HashSet<Kpiprogram>();
            Surveys = new HashSet<Survey>();
        }

        [Key]
        [Column("KPI_ID")]
        public int KpiId { get; set; }
        [Column("KPICode")]
        [StringLength(50)]
        public string Kpicode { get; set; }
        [Column("KPIName")]
        [StringLength(50)]
        public string Kpiname { get; set; }
        [StringLength(500)]
        public string Description { get; set; }

        [InverseProperty(nameof(Kpiprogram.Kpi))]
        public virtual ICollection<Kpiprogram> Kpiprograms { get; set; }
        [InverseProperty(nameof(Survey.Kpi))]
        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
