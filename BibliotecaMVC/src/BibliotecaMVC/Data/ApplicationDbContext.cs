using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //// Gera Chave Primaria Composta
            builder.Entity<LivroAutor>()
                .HasKey(bc => new { bc.AutorID, bc.LivroID });

            builder.Entity<LivroAutor>()
                .HasOne(bc => bc.Autor)
                .WithMany(b => b.LivroAutor)
                .HasForeignKey(bc => bc.AutorID);

            builder.Entity<LivroAutor>()
                .HasOne(bc => bc.Livro)
                .WithMany(c => c.LivroAutor)
                .HasForeignKey(bc => bc.LivroID);

            builder.Entity<LivroEmprestimo>()
                .HasKey(bc => new { bc.LivroID, bc.EmprestimoID });

            builder.Entity<LivroEmprestimo>()
                .HasOne(bc => bc.Livro)
                .WithMany(b => b.LivroEmprestimo)
                .HasForeignKey(bc => bc.LivroID);

            builder.Entity<LivroEmprestimo>()
                .HasOne(bc => bc.Emprestimo)
                .WithMany(c => c.LivroEmprestimo)
                .HasForeignKey(bc => bc.EmprestimoID);

            // SistemaUsuario
            builder.Entity<SistemaUsuario>()
                .HasKey(bc => new { bc.SistemaID, bc.UsuarioID });

            builder.Entity<SistemaUsuario>()
                .HasOne(bc => bc.Sistemas)
                .WithMany(b => b.SistemaUsuarios)
                .HasForeignKey(bc => bc.SistemaID);

            builder.Entity<SistemaUsuario>()
                .HasOne(bc => bc.Usuarios)
                .WithMany(c => c.SistemaUsuario)
                .HasForeignKey(bc => bc.UsuarioID);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            base.OnModelCreating(builder);
        }

        public DbSet<Livro> Livro { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Emprestimo> Emprestimo { get; set; }

        public DbSet<LivroAutor> LivroAutor { get; set; }

        public DbSet<Autor> Autor { get; set; }

        public DbSet<Sistema> Sistema { get; set; }

        public DbSet<LivroEmprestimo> LivroEmprestimo { get; set; }

        public DbSet<SistemaUsuario> SistemaUsuario { get; set; }
    }
}
