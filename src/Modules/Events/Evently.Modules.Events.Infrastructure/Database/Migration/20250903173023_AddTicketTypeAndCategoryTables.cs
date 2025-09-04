using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Modules.Events.Api.Database.Migration;

/// <inheritdoc />
public partial class AddTicketTypeAndCategoryTables : Microsoft.EntityFrameworkCore.Migrations.Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "categories",
            schema: "event",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_categories", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "ticket_types",
            schema: "event",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                event_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                currency = table.Column<string>(type: "text", nullable: false),
                price = table.Column<decimal>(type: "numeric", nullable: false),
                quantity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_ticket_types", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "categories",
            schema: "event");

        migrationBuilder.DropTable(
            name: "ticket_types",
            schema: "event");
    }
}
