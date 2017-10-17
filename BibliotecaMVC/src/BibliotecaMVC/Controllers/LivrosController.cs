using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BibliotecaMVC.Controllers
{
    public class LivrosController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        private readonly ApplicationDbContext _context;

        public LivrosController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        // GET: Livros
        public async Task<IActionResult> Index(string filtroPesquisa, string quantidadePesquisa, string ordenacao, string ordenacaoQtde, string filtroAtual)
        {
            ViewBag.filtroAtual = filtroAtual ?? "";
            ViewBag.TituloSortParm = string.IsNullOrEmpty(ordenacao) ? "titulo_desc" : "";
            ViewBag.QuantidadeSortParm = string.IsNullOrEmpty(ordenacaoQtde) ? "qt_desc" : "";

            ViewBag.filtroPesquisa = filtroPesquisa;

            ViewBag.quantidadePesquisa = quantidadePesquisa;



            var livros = from l in _context.Livro
                         select l;

            if (!String.IsNullOrEmpty(filtroPesquisa))
            {
                livros = livros.Where(s => s.Titulo.ToUpper().Contains(filtroPesquisa.ToUpper()));
            }

            if (!string.IsNullOrEmpty(quantidadePesquisa))
            {
                int qt;
                if (int.TryParse(quantidadePesquisa, out qt))
                {
                    livros = livros.Where(s => s.Quantidade >= qt);
                }
            }

            if (ViewBag.filtroAtual.Equals("qtde"))
            {
                switch (ordenacaoQtde)
                {
                    case "qt_desc":
                        livros = livros.OrderByDescending(s => s.Quantidade);
                        break;
                    default:
                        livros = livros.OrderBy(s => s.Quantidade);
                        break;
                }
            }
            else if (ViewBag.filtroAtual.Equals("titulo"))
            {
                switch (ordenacao)
                {
                    case "titulo_desc":
                        livros = livros.OrderByDescending(s => s.Titulo);
                        break;
                    default:
                        livros = livros.OrderBy(s => s.Titulo);
                        break;
                }
            }

            return View(await livros.ToListAsync());
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livro.AsNoTracking()
                              .Include(l => l.LivroAutor)
                              .ThenInclude(li => li.Autor)
                              .SingleOrDefaultAsync(m => m.LivroID == id);

            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        // GET: Livros/Create
        public IActionResult Create()
        {
            ViewBag.Autores = new Listagens(_context).AutoresCheckBox();

            return View(new Livro());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LivroID,Foto,Quantidade,Titulo,AutorUnico")] Livro livro
            , string[] selectedAutores, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (selectedAutores != null)
                {
                    livro.LivroAutor = new List<LivroAutor>();
                    foreach (var idAutor in selectedAutores)
                        livro.LivroAutor.Add(new LivroAutor()
                        {
                            AutorID =
                        int.Parse(idAutor),
                            Livro = livro
                        });
                }
                _context.Add(livro);
                await _context.SaveChangesAsync();
                livro.Foto = await RealizarUploadImagens(files, livro.LivroID);

                _context.Update(livro);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(livro);
        }

        private async Task<string> RealizarUploadImagens(List<IFormFile> files, int idLivro)
        {
            // Verifica se existem arquivos selecionados
            if (files.Count > 0)
            {
                // Variável para armazenar o caminho de upload das imagens
                var pathUpload = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                // Se o caminho não existe então cria
                if (!Directory.Exists(pathUpload))
                    Directory.CreateDirectory(pathUpload);
                // Para cada arquivo faça
                foreach (var file in files)
                {
                    // Verifica se o arquivo possui informação
                    if (file.Length > 0)
                    {
                        // Concatena o nome do arquivo
                        var nomeArquivo = "livro_" + idLivro +
                        Path.GetExtension(file.FileName);
                        // Concatena o caminho do arquivo
                        var pathFile = Path.Combine(pathUpload, nomeArquivo);
                        // Realiza a cópia
                        using (var fileStream = new FileStream(pathFile,
                        FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        // Retorna o caminho do arquivo que será salvo
                        // no banco de dados
                        return "uploads//" + Path.GetFileName(pathFile);
                    }
                }
            }
            return null;
        }

        // GET: Livros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autoresAux = new Listagens(_context).AutoresCheckBox();

            var livro = await _context.Livro.Include(l => l.LivroAutor)
                .SingleOrDefaultAsync(m => m.LivroID == id);

            autoresAux.ForEach(a =>
                                    a.Checked = livro.LivroAutor.Any(l => l.AutorID == a.Value)
                              );

            ViewBag.Autores = autoresAux;

            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("LivroID,Foto,Quantidade,Titulo")] Livro livro, string[] selectedAutores,
            List<IFormFile> files)
        {
            if (id != livro.LivroID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var livroAutores =
                    _context.LivroAutor.AsNoTracking().Where(la => la.LivroID == livro.LivroID);
                    _context.LivroAutor.RemoveRange(livroAutores);

                    await _context.SaveChangesAsync();

                    if (selectedAutores != null)
                    {
                        livro.LivroAutor = new List<LivroAutor>();
                        foreach (var idAutor in selectedAutores)
                            livro.LivroAutor.Add(new LivroAutor()
                            {
                                AutorID =
                            int.Parse(idAutor),
                                Livro = livro
                            });
                    }

                    livro.Foto = await RealizarUploadImagens(files, livro.LivroID);

                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivroExists(livro.LivroID))

                    {

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(livro);
        }

        // GET: Livros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livro.SingleOrDefaultAsync(m => m.LivroID == id);
            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livro = await _context.Livro.SingleOrDefaultAsync(m => m.LivroID == id);
            _context.Livro.Remove(livro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool LivroExists(int id)
        {
            return _context.Livro.Any(e => e.LivroID == id);
        }

    }
}
