using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    interface IRepositorioContrato : IRepositorio<Contrato>
    {
        IList<Contrato> ObtenerPorInmuebleId(int id);
        IList<Contrato> ObtenerTodosVigentes(DateTime fechaInicio, DateTime fechaFin);
    }

}
