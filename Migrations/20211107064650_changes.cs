using Microsoft.EntityFrameworkCore.Migrations;

namespace IP_KPI.Migrations
{
    public partial class changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaleStudent",
                table: "Student",
                newName: "NumberOfMaleStudent");

            migrationBuilder.RenameColumn(
                name: "FemaleStudent",
                table: "Student",
                newName: "NumberOfFemaleStudent");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Student",
                newName: "RecordNumber");

            migrationBuilder.RenameColumn(
                name: "ExternalTarget",
                table: "KPI",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "UniProgram",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FemaleSurveyResponses",
                table: "ProgramSurvey",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaleSurveyResponses",
                table: "ProgramSurvey",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Term",
                table: "KPI",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "KPI",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FemaleSurveyResponse",
                table: "CourseSurvey",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaleSurveyResponse",
                table: "CourseSurvey",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "UniProgram");

            migrationBuilder.DropColumn(
                name: "FemaleSurveyResponses",
                table: "ProgramSurvey");

            migrationBuilder.DropColumn(
                name: "MaleSurveyResponses",
                table: "ProgramSurvey");

            migrationBuilder.DropColumn(
                name: "Term",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "FemaleSurveyResponse",
                table: "CourseSurvey");

            migrationBuilder.DropColumn(
                name: "MaleSurveyResponse",
                table: "CourseSurvey");

            migrationBuilder.RenameColumn(
                name: "NumberOfMaleStudent",
                table: "Student",
                newName: "MaleStudent");

            migrationBuilder.RenameColumn(
                name: "NumberOfFemaleStudent",
                table: "Student",
                newName: "FemaleStudent");

            migrationBuilder.RenameColumn(
                name: "RecordNumber",
                table: "Student",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "KPI",
                newName: "ExternalTarget");
        }
    }
}
