using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBoardManagement.Migrations
{
    public partial class SeedProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Example UserIds from your screenshot
            var manager1 = Guid.Parse("632AA344-35CC-41B1-B2E3-95EA4F4ADA39");
            var manager2 = Guid.Parse("0223EE12-3043-474A-B124-BA78135A20A9");
            var dev1 = Guid.Parse("8DCB621E-F264-48C2-8624-63F39ECF4054");
            var dev2 = Guid.Parse("F9806404-424D-4591-90F2-96FE694FE30B");

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Name", "Description", "OwnerId" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Recruitment Portal", "Job application tracking system", manager1 },
                    { Guid.NewGuid(), "Sales Dashboard", "Real-time sales analytics tool", manager2 },
                    { Guid.NewGuid(), "Chat App", "Messaging application for teams", dev1 },
                    { Guid.NewGuid(), "IoT Monitor", "Sensor data collection and visualization", dev2 },
                    { Guid.NewGuid(), "TaskBoard Core", "Main backend for task management", manager1 },
                    { Guid.NewGuid(), "API Gateway", "Centralized API gateway service", dev1 }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Projects]");
        }
    }
}
