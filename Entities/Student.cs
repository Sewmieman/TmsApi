namespace TmsApi.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
public class Student
{
public int Id { get; set; }
// surrogate primary key — internal, used by foreign keys
public required string RegistrationNumber { get; set; } // natural key — human-readable (uniqueness configured in Session 2)
public required string Name { get; set; }
public decimal GPA { get; set; }
public bool IsActive { get; set; } = true;
// Navigation property for many-to-many relationship
public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

}
public partial class AddStudentEmail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Students",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Email",
            table: "Students");
    }
}