using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudinaryTest.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFullAuditforCloudFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudFolders_CloudFolders_CloudFolderID",
                table: "CloudFolders");

            migrationBuilder.RenameColumn(
                name: "CloudFolderID",
                table: "CloudFolders",
                newName: "CloudFolderId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CloudFolders",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFolders_CloudFolderID",
                table: "CloudFolders",
                newName: "IX_CloudFolders_CloudFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudFolders_CloudFolders_CloudFolderId",
                table: "CloudFolders",
                column: "CloudFolderId",
                principalTable: "CloudFolders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudFolders_CloudFolders_CloudFolderId",
                table: "CloudFolders");

            migrationBuilder.RenameColumn(
                name: "CloudFolderId",
                table: "CloudFolders",
                newName: "CloudFolderID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CloudFolders",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_CloudFolders_CloudFolderId",
                table: "CloudFolders",
                newName: "IX_CloudFolders_CloudFolderID");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudFolders_CloudFolders_CloudFolderID",
                table: "CloudFolders",
                column: "CloudFolderID",
                principalTable: "CloudFolders",
                principalColumn: "ID");
        }
    }
}
