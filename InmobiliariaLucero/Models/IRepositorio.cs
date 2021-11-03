﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    interface IRepositorio<T>
    {

        int Alta(T p);
        int Baja(int id);
        int Modificacion(T p);
        IList<T> ObtenerTodos();
        T ObtenerPorId(int id);
    }
}
