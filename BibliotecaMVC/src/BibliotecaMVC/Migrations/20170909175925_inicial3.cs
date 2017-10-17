using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibliotecaMVC.Migrations
{
    public partial class inicial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInicio",
                table: "Emprestimo",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFim",
                table: "Emprestimo",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucao",
                table: "Emprestimo",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DataInicio",
                table: "Emprestimo",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataFim",
                table: "Emprestimo",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataDevolucao",
                table: "Emprestimo",
                nullable: true);
        }
    }
}
