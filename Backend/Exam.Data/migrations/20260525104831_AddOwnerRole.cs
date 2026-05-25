using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.migrations
{
    /// <inheritdoc />
    public partial class AddOwnerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE `User`
                SET `Role` = 'Owner'
                WHERE `Role` = 'Admin'
                ORDER BY `Id`
                LIMIT 1;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE `User`
                SET `Role` = 'Admin'
                WHERE `Role` = 'Owner';
                """);
        }
    }
}
