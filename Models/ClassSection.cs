using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace IP_KPI.Models
{
    [Table("ClassSection")]
    public partial class ClassSection
    {
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(50)]
        public string Gender { get; set; }
        [StringLength(50)]
        public string Term { get; set; }
        public int? NumOfClasses { get; set; }
        [Key]
        [Column("SectionID")]
        public int SectionId { get; set; }
        [Column("programId")]
        public int? ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        [InverseProperty(nameof(UniProgram.ClassSections))]
        public virtual UniProgram Program { get; set; }
    }
}
