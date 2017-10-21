using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibliotecaMVC.Migrations
{
    public partial class Datadevolucao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucao",
                table: "Emprestimo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucao",
                table: "Emprestimo",
                nullable: false);
        }
    }
}
