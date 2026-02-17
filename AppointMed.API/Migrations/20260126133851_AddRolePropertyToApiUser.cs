using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointMed.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePropertyToApiUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "113",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedUserName", "PasswordHash", "Role", "SecurityStamp", "UserName" },
                values: new object[] { "c8dfe518-6f8e-4296-8952-4ca6dbda7215", true, "ADMIN@APPOINTMED.COM", "AQAAAAIAAYagAAAAEDSKN5q/1Q3ucVvmXgkP0u7OM9GCdm/B/FSSeJCiXslmHNfkz5fm2xxs6BHySMigeA==", "Administrator", "4e5332e6-4c66-4d47-97bd-80d01db01faf", "admin@appointmed.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "114",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedUserName", "PasswordHash", "Role", "SecurityStamp", "UserName" },
                values: new object[] { "22e070ae-04ed-448a-b5d9-44355c1b25ae", true, "USER@APPOINTMED.COM", "AQAAAAIAAYagAAAAEAA5/cNjtHRLiUE0uipz31fABfHFYDOBnyNm2BQBIvSolNd4axyzLFXt2NUF65+k/A==", "User", "58975c52-90e4-4832-b90a-f339f7020a17", "user@appointmed.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "113",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "80b37696-22a8-45f0-8dfa-14bce3f81b43", false, "ADMIN@123", null, "2bd1df8b-1ced-4796-ad2a-81911622f088", "admin@123" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "114",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "b26ee874-d407-4468-99f1-a9769bce01f2", false, "USER@123", null, "2a8d5165-033c-4275-a2c6-0a32b9fa5872", "user@123" });
        }
    }
}
