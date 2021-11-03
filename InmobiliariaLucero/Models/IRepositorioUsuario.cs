using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario ObtenerPorEmail(string email);
        IList<Usuario> BuscarPorNombre(string nombre);

    }

}
