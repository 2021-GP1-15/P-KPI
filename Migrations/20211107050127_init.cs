using Microsoft.EntityFrameworkCore.Migrations;

namespace IP_KPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "College",
                columns: table => new
                {
                    CollegeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_College", x => x.CollegeID);
                });

            migrationBuilder.CreateTable(
                name: "KPI",
                columns: table => new
                {
                    KPI_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KPICode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KPIName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExternalTarget = table.Column<double>(type: "float", nullable: true),
                    ActualTarget = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPI", x => x.KPI_ID);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FemaleFaculty = table.Column<int>(type: "int", nullable: true),
                    MaleFaculty = table.Column<int>(type: "int", nullable: true),
                    CollegeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Department_College",
                        column: x => x.CollegeID,
                        principalTable: "College",
                        principalColumn: "CollegeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UniProgram",
                columns: table => new
                {
                    ProgramID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreditHour = table.Column<int>(type: "int", nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Program", x => x.ProgramID);
                    table.ForeignKey(
                        name: "FK_UniProgram_Department",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditHour = table.Column<int>(type: "int", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseID);
                    table.ForeignKey(
                        name: "FK_Course_Program",
                        column: x => x.ProgramID,
                        principalTable: "UniProgram",
                        principalColumn: "ProgramID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramSurvey",
                columns: table => new
                {
                    SurveyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PSFemaleScore = table.Column<double>(type: "float", nullable: true),
                    PSMaleScore = table.Column<double>(type: "float", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: true),
                    KPI_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramSurvey", x => x.SurveyID);
                    table.ForeignKey(
                        name: "FK_ProgramSurvey_KPI",
                        column: x => x.KPI_ID,
                        principalTable: "KPI",
                        principalColumn: "KPI_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramSurvey_Program",
                        column: x => x.ProgramID,
                        principalTable: "UniProgram",
                        principalColumn: "ProgramID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    FemaleStudent = table.Column<int>(type: "int", nullable: true),
                    MaleStudent = table.Column<int>(type: "int", nullable: true),
                    CourseID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Student_Course",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseSurvey",
                columns: table => new
                {
                    SurveyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CSFemaleScore = table.Column<double>(type: "float", nullable: true),
                    CSMaleScore = table.Column<double>(type: "float", nullable: true),
                    CourseID = table.Column<int>(type: "int", nullable: true),
                    KPI_ID = table.Column<int>(type: "int", nullable: true),
                    ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSurvey", x => x.SurveyID);
                    table.ForeignKey(
                        name: "FK_CourseSurvey_Course",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSurvey_KPI",
                        column: x => x.KPI_ID,
                        principalTable: "KPI",
                        principalColumn: "KPI_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSurvey_Student",
                        column: x => x.ID,
                        principalTable: "Student",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ProgramID",
                table: "Course",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSurvey_CourseID",
                table: "CourseSurvey",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSurvey_ID",
                table: "CourseSurvey",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSurvey_KPI_ID",
                table: "CourseSurvey",
                column: "KPI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CollegeID",
                table: "Department",
                column: "CollegeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSurvey_KPI_ID",
                table: "ProgramSurvey",
                column: "KPI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSurvey_ProgramID",
                table: "ProgramSurvey",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_CourseID",
                table: "Student",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_UniProgram_DepartmentID",
                table: "UniProgram",
                column: "DepartmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSurvey");

            migrationBuilder.DropTable(
                name: "ProgramSurvey");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "KPI");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "UniProgram");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "College");
        }
    }
}
