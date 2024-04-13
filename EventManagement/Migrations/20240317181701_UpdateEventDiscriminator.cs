using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Migrations
{
    public partial class UpdateEventDiscriminator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Events SET Discriminator = 'AdminEvent' WHERE Discriminator = 'ConcertEvent'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Events SET Discriminator = 'ConcertEvent' WHERE Discriminator = 'AdminEvent'");
        }
    }

}
