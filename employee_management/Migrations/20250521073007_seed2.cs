using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace employee_management.Migrations
{
    /// <inheritdoc />
    public partial class seed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "894dac4f-d387-40a8-933f-c64be4e62684", "admin@admin.com", "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAEAUouA3CVTJWV2CCBm7NwlSj4S3J5rxlc3BFHufV5IfN6e5kXoHMw4wWXKoAyKt+GA==", "73e7f3f3-9573-453b-9e37-0f234c2723fc", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "39306f99-5bad-43f7-8624-bb8377377a2e", "admin@hospital.com", "ADMIN@HOSPITAL.COM", "ADMIN@HOSPITAL.COM", "AQAAAAIAAYagAAAAEEjp0dSOMy99tIR/fESXB31xC4smCVBg5hatR1IpthjER1NOlo8haJrLyWTQ9Q5ZJA==", "280e55d9-b565-4498-928a-5410aebfc42f", "admin@hospital.com" });
        }
    }
}
