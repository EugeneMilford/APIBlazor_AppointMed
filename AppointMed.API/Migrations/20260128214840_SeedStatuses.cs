using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointMed.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "113",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d4ece603-053d-4afe-bec5-6b960d083613", "AQAAAAIAAYagAAAAEH95/J/57Ix1R3u/luEMBP71FPqa4lZfT2WcDJGHkDJBrkLy3R1jKfVEe2RTulpH1g==", "294a7665-054e-438a-888f-1bd5aef67507" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "114",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "decbc5b6-2869-439a-867d-87cd5da2ef27", "AQAAAAIAAYagAAAAEM8wPkaPcM/YAJEScwxRnprs09AxIjAtnKutJqsXcPdLcxswWDyI2se2tmie7NRTOg==", "5333c86e-7361-4028-9bdb-18bdfd679c4b" });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "StatusId", "DisplayOrder", "IsActive", "StatusDescription", "StatusName" },
                values: new object[,]
                {
                    { 1, 1, true, "Appointment has been scheduled", "Scheduled" },
                    { 2, 2, true, "Patient has confirmed the appointment", "Confirmed" },
                    { 3, 3, true, "Appointment has been completed", "Completed" },
                    { 4, 4, true, "Appointment has been cancelled", "Cancelled" },
                    { 5, 5, true, "Patient did not show up for appointment", "No Show" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "StatusId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "113",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8dfe518-6f8e-4296-8952-4ca6dbda7215", "AQAAAAIAAYagAAAAEDSKN5q/1Q3ucVvmXgkP0u7OM9GCdm/B/FSSeJCiXslmHNfkz5fm2xxs6BHySMigeA==", "4e5332e6-4c66-4d47-97bd-80d01db01faf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "114",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22e070ae-04ed-448a-b5d9-44355c1b25ae", "AQAAAAIAAYagAAAAEAA5/cNjtHRLiUE0uipz31fABfHFYDOBnyNm2BQBIvSolNd4axyzLFXt2NUF65+k/A==", "58975c52-90e4-4832-b90a-f339f7020a17" });
        }
    }
}
