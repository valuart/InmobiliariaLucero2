using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class Contrato
    {
        [Key]
        [DisplayName("Codigo")]
        public int Id { get; set; }
        [Required, Display(Name = "Locador")]
        public Inmueble Inmueble { get; set; }
        public int IdInmu { get; set; }
        [Required, Display(Name = "Locatario")]
        public Inquilino Inquilino { get; set; }
        public int IdInqui { get; set; }
        [Required, Display(Name = "Fecha Inicio"), DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha Fin"), DataType(DataType.Date)] 
        public DateTime FechaFin { get; set; }
        [Required, Display(Name = "Importe")]
        public decimal Monto { get; set; }
        public bool Estado { get; set; }

    }
}
