﻿// <auto-generated />
using System;
using IP_KPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IP_KPI.Migrations
{
    [DbContext(typeof(db_a7baa5_ipkpiContext))]
    [Migration("20211107050127_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IP_KPI.Models.College", b =>
                {
                    b.Property<int>("CollegeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CollegeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CollageName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CollegeId");

                    b.ToTable("College");
                });

            modelBuilder.Entity("IP_KPI.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CourseID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("CreditHour")
                        .HasColumnType("int");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramID");

                    b.HasKey("CourseId");

                    b.HasIndex("ProgramId");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("IP_KPI.Models.CourseSurvey", b =>
                {
                    b.Property<int>("SurveyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SurveyID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CourseId")
                        .HasColumnType("int")
                        .HasColumnName("CourseID");

                    b.Property<double?>("CsfemaleScore")
                        .HasColumnType("float")
                        .HasColumnName("CSFemaleScore");

                    b.Property<double?>("CsmaleScore")
                        .HasColumnType("float")
                        .HasColumnName("CSMaleScore");

                    b.Property<int?>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int?>("KpiId")
                        .HasColumnType("int")
                        .HasColumnName("KPI_ID");

                    b.HasKey("SurveyId");

                    b.HasIndex("CourseId");

                    b.HasIndex("Id");

                    b.HasIndex("KpiId");

                    b.ToTable("CourseSurvey");
                });

            modelBuilder.Entity("IP_KPI.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CollegeId")
                        .HasColumnType("int")
                        .HasColumnName("CollegeID");

                    b.Property<string>("DepartmentName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("FemaleFaculty")
                        .HasColumnType("int");

                    b.Property<int?>("MaleFaculty")
                        .HasColumnType("int");

                    b.HasKey("DepartmentId");

                    b.HasIndex("CollegeId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("IP_KPI.Models.Kpi", b =>
                {
                    b.Property<int>("KpiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("KPI_ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("ActualTarget")
                        .HasColumnType("float");

                    b.Property<double?>("ExternalTarget")
                        .HasColumnType("float");

                    b.Property<string>("Kpicode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("KPICode");

                    b.Property<string>("Kpiname")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("KPIName");

                    b.HasKey("KpiId");

                    b.ToTable("KPI");
                });

            modelBuilder.Entity("IP_KPI.Models.ProgramSurvey", b =>
                {
                    b.Property<int>("SurveyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SurveyID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("KpiId")
                        .HasColumnType("int")
                        .HasColumnName("KPI_ID");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramID");

                    b.Property<double?>("PsfemaleScore")
                        .HasColumnType("float")
                        .HasColumnName("PSFemaleScore");

                    b.Property<double?>("PsmaleScore")
                        .HasColumnType("float")
                        .HasColumnName("PSMaleScore");

                    b.HasKey("SurveyId");

                    b.HasIndex("KpiId");

                    b.HasIndex("ProgramId");

                    b.ToTable("ProgramSurvey");
                });

            modelBuilder.Entity("IP_KPI.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CourseId")
                        .HasColumnType("int")
                        .HasColumnName("CourseID");

                    b.Property<int?>("FemaleStudent")
                        .HasColumnType("int");

                    b.Property<int?>("MaleStudent")
                        .HasColumnType("int");

                    b.Property<string>("Term")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("IP_KPI.Models.UniProgram", b =>
                {
                    b.Property<int>("ProgramId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProgramID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreditHour")
                        .HasColumnType("int");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<string>("ProgramName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProgramId")
                        .HasName("PK_Program");

                    b.HasIndex("DepartmentId");

                    b.ToTable("UniProgram");
                });

            modelBuilder.Entity("IP_KPI.Models.Course", b =>
                {
                    b.HasOne("IP_KPI.Models.UniProgram", "Program")
                        .WithMany("Courses")
                        .HasForeignKey("ProgramId")
                        .HasConstraintName("FK_Course_Program");

                    b.Navigation("Program");
                });

            modelBuilder.Entity("IP_KPI.Models.CourseSurvey", b =>
                {
                    b.HasOne("IP_KPI.Models.Course", "Course")
                        .WithMany("CourseSurveys")
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_CourseSurvey_Course");

                    b.HasOne("IP_KPI.Models.Student", "IdNavigation")
                        .WithMany("CourseSurveys")
                        .HasForeignKey("Id")
                        .HasConstraintName("FK_CourseSurvey_Student");

                    b.HasOne("IP_KPI.Models.Kpi", "Kpi")
                        .WithMany("CourseSurveys")
                        .HasForeignKey("KpiId")
                        .HasConstraintName("FK_CourseSurvey_KPI");

                    b.Navigation("Course");

                    b.Navigation("IdNavigation");

                    b.Navigation("Kpi");
                });

            modelBuilder.Entity("IP_KPI.Models.Department", b =>
                {
                    b.HasOne("IP_KPI.Models.College", "College")
                        .WithMany("Departments")
                        .HasForeignKey("CollegeId")
                        .HasConstraintName("FK_Department_College");

                    b.Navigation("College");
                });

            modelBuilder.Entity("IP_KPI.Models.ProgramSurvey", b =>
                {
                    b.HasOne("IP_KPI.Models.Kpi", "Kpi")
                        .WithMany("ProgramSurveys")
                        .HasForeignKey("KpiId")
                        .HasConstraintName("FK_ProgramSurvey_KPI");

                    b.HasOne("IP_KPI.Models.UniProgram", "Program")
                        .WithMany("ProgramSurveys")
                        .HasForeignKey("ProgramId")
                        .HasConstraintName("FK_ProgramSurvey_Program");

                    b.Navigation("Kpi");

                    b.Navigation("Program");
                });

            modelBuilder.Entity("IP_KPI.Models.Student", b =>
                {
                    b.HasOne("IP_KPI.Models.Course", "Course")
                        .WithMany("Students")
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_Student_Course");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("IP_KPI.Models.UniProgram", b =>
                {
                    b.HasOne("IP_KPI.Models.Department", "Department")
                        .WithMany("UniPrograms")
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("FK_UniProgram_Department");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("IP_KPI.Models.College", b =>
                {
                    b.Navigation("Departments");
                });

            modelBuilder.Entity("IP_KPI.Models.Course", b =>
                {
                    b.Navigation("CourseSurveys");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("IP_KPI.Models.Department", b =>
                {
                    b.Navigation("UniPrograms");
                });

            modelBuilder.Entity("IP_KPI.Models.Kpi", b =>
                {
                    b.Navigation("CourseSurveys");

                    b.Navigation("ProgramSurveys");
                });

            modelBuilder.Entity("IP_KPI.Models.Student", b =>
                {
                    b.Navigation("CourseSurveys");
                });

            modelBuilder.Entity("IP_KPI.Models.UniProgram", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("ProgramSurveys");
                });
#pragma warning restore 612, 618
        }
    }
}
