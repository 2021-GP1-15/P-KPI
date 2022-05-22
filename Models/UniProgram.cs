using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("UniProgram")]
    public partial class UniProgram
    {
        public UniProgram()
        {
            AlumniEmployments = new HashSet<AlumniEmployment>();
            BatchStatistics = new HashSet<BatchStatistic>();
            ClassSections = new HashSet<ClassSection>();
            FacultyStatistics = new HashSet<FacultyStatistic>();
            Kpiprograms = new HashSet<Kpiprogram>();
            PublicationReports = new HashSet<PublicationReport>();
            Surveys = new HashSet<Survey>();
        }

        [Key]
        [Column("ProgramID")]
        public int ProgramId { get; set; }
        [StringLength(50)]
        public string ProgramName { get; set; }
        [Column("DepartmentID")]
        public int? DepartmentId { get; set; }
        [StringLength(50)]
        public string Level { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        [InverseProperty("UniPrograms")]
        public virtual Department Department { get; set; }
        [InverseProperty(nameof(AlumniEmployment.Program))]
        public virtual ICollection<AlumniEmployment> AlumniEmployments { get; set; }
        [InverseProperty(nameof(BatchStatistic.Program))]
        public virtual ICollection<BatchStatistic> BatchStatistics { get; set; }
        [InverseProperty(nameof(ClassSection.Program))]
        public virtual ICollection<ClassSection> ClassSections { get; set; }
        [InverseProperty(nameof(FacultyStatistic.Program))]
        public virtual ICollection<FacultyStatistic> FacultyStatistics { get; set; }
        [InverseProperty(nameof(Kpiprogram.Program))]
        public virtual ICollection<Kpiprogram> Kpiprograms { get; set; }
        [InverseProperty(nameof(PublicationReport.Program))]
        public virtual ICollection<PublicationReport> PublicationReports { get; set; }
        [InverseProperty(nameof(Survey.Program))]
        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
