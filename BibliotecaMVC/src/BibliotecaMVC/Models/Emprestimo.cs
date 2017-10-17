using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaMVC.Models
{
    public class Emprestimo
    {
        [Key]
        public int EmprestimoID { get; set; }

        [Display(Name = "Usuário")]
        public int UsuarioID { get; set; }
        public virtual Usuario Usuario { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de início")]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de fim")]
        public DateTime DataFim { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de devolução")]
        public DateTime DataDevolucao { get; set; }

        public ICollection<LivroEmprestimo> LivroEmprestimo { get; set; }
    }
}
