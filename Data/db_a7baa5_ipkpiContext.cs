using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using IP_KPI.Models;

#nullable disable

namespace IP_KPI.Data
{
    public partial class db_a7baa5_ipkpiContext : DbContext
    {
        public db_a7baa5_ipkpiContext()
        {
        }

        public db_a7baa5_ipkpiContext(DbContextOptions<db_a7baa5_ipkpiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AlumniEmployment> AlumniEmployments { get; set; }
        public virtual DbSet<BatchStatistic> BatchStatistics { get; set; }
        public virtual DbSet<ClassSection> ClassSections { get; set; }
        public virtual DbSet<College> Colleges { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<FacultyStatistic> FacultyStatistics { get; set; }
        public virtual DbSet<Kpi> Kpis { get; set; }
        public virtual DbSet<Kpiprogram> Kpiprograms { get; set; }
        public virtual DbSet<PrivilegeRequest> PrivilegeRequests { get; set; }
        public virtual DbSet<PublicationReport> PublicationReports { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<UniProgram> UniPrograms { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("data source=SQL5104.site4now.net;initial catalog=db_a7baa5_ipkpi;user id=db_a7baa5_ipkpi_admin;password=GP123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AlumniEmployment>(entity =>
            {
                entity.HasKey(e => e.AlumniId)
                    .HasName("AlumniID");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.AlumniEmployments)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_AlumniEmployment_UniProgram");
            });

            modelBuilder.Entity<BatchStatistic>(entity =>
            {
                entity.HasKey(e => e.BatchId)
                    .HasName("BatchID");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.BatchStatistics)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_BatchStatistic_UniProgram");
            });

            modelBuilder.Entity<ClassSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("SectionID");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.ClassSections)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_ClassSection_UniProgram");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasOne(d => d.College)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.CollegeId)
                    .HasConstraintName("FK_Department_College");
            });

            modelBuilder.Entity<FacultyStatistic>(entity =>
            {
                entity.HasKey(e => e.FacultyId)
                    .HasName("PK_FacultyStatisics");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.FacultyStatistics)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_FacultyStatisics_UniProgram");
            });

            modelBuilder.Entity<Kpiprogram>(entity =>
            {
                entity.HasOne(d => d.Kpi)
                    .WithMany(p => p.Kpiprograms)
                    .HasForeignKey(d => d.KpiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KPIProgram_KPI");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.Kpiprograms)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KPIProgram_UniProgram");
            });

            modelBuilder.Entity<PrivilegeRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK_PrivilegesReq");

                entity.Property(e => e.UserId).IsUnicode(false);
            });

            modelBuilder.Entity<PublicationReport>(entity =>
            {
                entity.HasOne(d => d.Program)
                    .WithMany(p => p.PublicationReports)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_Faculty_UniProgram");
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasKey(e => e.RecordNumber)
                    .HasName("PK_Student");

                entity.HasOne(d => d.Kpi)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.KpiId)
                    .HasConstraintName("FK_StudentSurvey_KPI");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.Surveys)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_StudentSurvey_UniProgram");
            });

            modelBuilder.Entity<UniProgram>(entity =>
            {
                entity.HasKey(e => e.ProgramId)
                    .HasName("PK_Program");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.UniPrograms)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_UniProgram_Department");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
