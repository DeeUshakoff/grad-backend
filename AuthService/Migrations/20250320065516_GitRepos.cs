using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class GitRepos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitRepository_Projects_ProjectId",
                table: "GitRepository");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskGitRelations_GitRepository_GitRepositoryId",
                table: "TaskGitRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GitRepository",
                table: "GitRepository");

            migrationBuilder.RenameTable(
                name: "GitRepository",
                newName: "GitRepositories");

            migrationBuilder.RenameIndex(
                name: "IX_GitRepository_ProjectId",
                table: "GitRepositories",
                newName: "IX_GitRepositories_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitRepositories",
                table: "GitRepositories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GitRepositories_Projects_ProjectId",
                table: "GitRepositories",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGitRelations_GitRepositories_GitRepositoryId",
                table: "TaskGitRelations",
                column: "GitRepositoryId",
                principalTable: "GitRepositories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitRepositories_Projects_ProjectId",
                table: "GitRepositories");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskGitRelations_GitRepositories_GitRepositoryId",
                table: "TaskGitRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GitRepositories",
                table: "GitRepositories");

            migrationBuilder.RenameTable(
                name: "GitRepositories",
                newName: "GitRepository");

            migrationBuilder.RenameIndex(
                name: "IX_GitRepositories_ProjectId",
                table: "GitRepository",
                newName: "IX_GitRepository_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitRepository",
                table: "GitRepository",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GitRepository_Projects_ProjectId",
                table: "GitRepository",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGitRelations_GitRepository_GitRepositoryId",
                table: "TaskGitRelations",
                column: "GitRepositoryId",
                principalTable: "GitRepository",
                principalColumn: "Id");
        }
    }
}
