using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> ObtenerTodosPorPropietarioId(int propietarioId);
        IList<Inmueble> ObtenerTodosDisponibles(DateTime fechaInicio, DateTime fechaFinal);
    }

}
