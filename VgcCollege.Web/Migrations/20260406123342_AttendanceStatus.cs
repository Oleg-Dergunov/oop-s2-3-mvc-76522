using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.Web.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Present",
                table: "AttendanceRecords",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AttendanceRecords",
                newName: "Present");
        }
    }
}
