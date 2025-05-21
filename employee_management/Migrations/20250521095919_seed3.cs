using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace employee_management.Migrations
{
    /// <inheritdoc />
    public partial class seed3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "944dfc1f-e47f-463e-b44b-c6637e87d7bd", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAECq6yNnkKbahA7EY3Ug5SZ41KiNJwnwCIiTGivo34f5c+LPm/nF2+jJMBWSzqNwqFQ==", "aec807a7-496d-4b29-a451-66738c32d426", "admin@admin.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "894dac4f-d387-40a8-933f-c64be4e62684", "ADMIN", "AQAAAAIAAYagAAAAEAUouA3CVTJWV2CCBm7NwlSj4S3J5rxlc3BFHufV5IfN6e5kXoHMw4wWXKoAyKt+GA==", "73e7f3f3-9573-453b-9e37-0f234c2723fc", "admin" });
        }
    }
}
