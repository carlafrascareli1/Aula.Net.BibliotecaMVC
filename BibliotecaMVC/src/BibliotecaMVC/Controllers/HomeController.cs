using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Sistema para aluguel de livros";
            ViewData["NomeAluno"] = "Carla Frascareli";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contato da aluna";
            ViewData["NomeAluno"] = "Carla Frascareli";
            ViewData["EmailAluno"] = "carlafrascareli@gmail.com";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
