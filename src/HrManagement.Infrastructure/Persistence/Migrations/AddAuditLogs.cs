using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManagement.Infrastructure.Persistence.Migrations;

public partial class AddAuditLogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "audit_logs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Action = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Resource = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                User = table.Column<string>(type: "text", nullable: true),
                Details = table.Column<string>(type: "text", nullable: false),
                Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_audit_logs", x => x.Id); });

        migrationBuilder.CreateIndex(
            name: "ix_audit_logs_timestamp",
            table: "audit_logs",
            column: "Timestamp");

        migrationBuilder.CreateIndex(
            name: "ix_audit_logs_action",
            table: "audit_logs",
            column: "Action");

        migrationBuilder.CreateIndex(
            name: "ix_audit_logs_resource",
            table: "audit_logs",
            column: "Resource");

        migrationBuilder.CreateIndex(
            name: "ix_audit_logs_user",
            table: "audit_logs",
            column: "User");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "audit_logs");
    }
}
