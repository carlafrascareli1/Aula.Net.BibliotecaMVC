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
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmprestimoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Emprestimo
        public async Task<IActionResult> Index()
        {
            List<Emprestimo> listaEmprestimos = new List<Emprestimo>();
            // Verificamos se o usu�rio est� logado
            if (User.Identity.IsAuthenticated)
            {
                // Pegar ID do Usu�rio
                string userID = _userManager.GetUserId(HttpContext.User);
                listaEmprestimos = await _context.Emprestimo.Include(e => e.Usuario)
                .Where(c => c.ApplicationUserId == userID)

                .ToListAsync();

            }
            return View("Index", listaEmprestimos);
        }

        // GET: Emprestimo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimo
                 .Include(e => e.LivroEmprestimo)
                 .ThenInclude(le => le.Livro)
                 .ThenInclude(li => li.LivroAutor)
                 .ThenInclude(la => la.Autor)
                 .SingleOrDefaultAsync(m => m.EmprestimoID == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        // GET: Emprestimo/Create
        public IActionResult Create()
        {
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Email");
            ViewBag.Livros = new Listagens(_context).LivrosCheckBox();

            return View();
        }

        // POST: Emprestimo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmprestimoID,DataDevolucao,DataFim,DataInicio,UsuarioID")]
            Emprestimo emprestimo
            , string[] selectedLivros)
        {
            if (selectedLivros != null)
            {
                emprestimo.LivroEmprestimo = new List<LivroEmprestimo>();

                foreach (var id in selectedLivros)
                {
                    emprestimo.LivroEmprestimo.Add(new LivroEmprestimo
                    {
                        Emprestimo = emprestimo,
                        LivroID = Convert.ToInt32(id)
                    });
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(emprestimo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Email", emprestimo.UsuarioID);
            return View(emprestimo);
        }

        // GET: Emprestimo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimo
                .Include(p=>p.LivroEmprestimo)
                .SingleOrDefaultAsync(m => m.EmprestimoID == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            var livrosAux = new Listagens(_context).LivrosCheckBox();

            livrosAux.ForEach(a =>
                                    a.Checked = emprestimo.LivroEmprestimo.Any(l => l.LivroID == a.Value)
                              );

            ViewBag.Livros = livrosAux;

            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Email", emprestimo.UsuarioID);

            return View(emprestimo);
        }

        // POST: Emprestimo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id
            , [Bind("EmprestimoID,DataDevolucao,DataFim,DataInicio,UsuarioID")] Emprestimo emprestimo
            , string[] selectedLivros
            )
        {
            if (id != emprestimo.EmprestimoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.LivroEmprestimo.RemoveRange(
                        _context.LivroEmprestimo.AsNoTracking().Where(p => p.EmprestimoID == id));
                    await _context.SaveChangesAsync();

                    if (selectedLivros != null)
                    {
                        emprestimo.LivroEmprestimo = new List<LivroEmprestimo>();

                        foreach (var idLivro in selectedLivros)
                            emprestimo.LivroEmprestimo.Add(new LivroEmprestimo() { LivroID = int.Parse(idLivro),  Emprestimo = emprestimo });
                    }


                    _context.Update(emprestimo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmprestimoExists(emprestimo.EmprestimoID))
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
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Email", emprestimo.UsuarioID);
            return View(emprestimo);
        }

        // GET: Emprestimo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimo.SingleOrDefaultAsync(m => m.EmprestimoID == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        // POST: Emprestimo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprestimo = await _context.Emprestimo.SingleOrDefaultAsync(m => m.EmprestimoID == id);
            _context.Emprestimo.Remove(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EmprestimoExists(int id)
        {
            return _context.Emprestimo.Any(e => e.EmprestimoID == id);
        }

        public async Task<IActionResult> DevolverLivros(int? id)
        {
            if (id != null)
            {
                Emprestimo emprestimo = _context.Emprestimo.FirstOrDefault(e =>
                e.EmprestimoID == id);
                emprestimo.DataDevolucao = DateTime.Now;
                _context.Update(emprestimo);
                _context.SaveChanges();
            }
            return await Index();
        }
    }
}
