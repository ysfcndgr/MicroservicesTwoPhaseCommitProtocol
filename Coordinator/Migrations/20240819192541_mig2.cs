using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("4212ace3-4175-42ec-8099-549308d3e071"), "Payment.API" },
                    { new Guid("54f23361-ccc9-43ff-9797-da5b605327a8"), "Order.API" },
                    { new Guid("b754e48f-b992-4dea-9e10-c714054d656b"), "Stock.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("4212ace3-4175-42ec-8099-549308d3e071"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("54f23361-ccc9-43ff-9797-da5b605327a8"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("b754e48f-b992-4dea-9e10-c714054d656b"));
        }
    }
}
