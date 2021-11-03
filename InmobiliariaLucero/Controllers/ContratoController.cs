using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Controllers
{

    
    public class ContratoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioContrato rc;
        private readonly RepositorioInmueble ri;
        private readonly RepositorioInquilino rinq;
        private readonly RepositorioPago rpa;

        public ContratoController(IConfiguration configuration)
        {
            rc = new RepositorioContrato(configuration);
            ri= new RepositorioInmueble(configuration);
            rinq = new RepositorioInquilino(configuration);
            rpa = new RepositorioPago(configuration);
            this.configuration = configuration;
        }
        // GET: ContratoController/Index
        public ActionResult Index(int id)
        {
            var lista = rc.ObtenerTodos();
            return View(lista);
        }

        // GET: ContratoController/Details

        public ActionResult Details(int id)
        {
            var contrato = rc.ObtenerPorId(id);
            return View(contrato);

        }

        // GET: ContratoController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Inmueble = ri.ObtenerTodos();
            ViewBag.Inquilino = rinq.ObtenerTodos();
            return View();
        }

        // POST: ContratoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Contrato c)
        {
           // var res = 0;
            try
            {
                int res = rc.Alta(c);
                Inquilino i = rinq.ObtenerPorId(c.IdInqui);
                TempData["Id"] = c.Id;
                return RedirectToAction(nameof(Index));

                /* if (ModelState.IsValid)
                 {
                     var contratos = rc.ObtenerPorInmuebleId(c.IdInmu);
                     var fechFin = c.FechaFin;
                     var fechIni = c.FechaInicio;
                     var count = contratos.Count;

                     IList<Contrato> ctosNuevos = new List<Contrato>();
                     foreach (var item in contratos)
                     {
                         if (item.Estado)
                         {
                             if (fechIni < item.FechaInicio && fechFin < item.FechaInicio)
                             {
                                 ctosNuevos.Add(item);
                             }
                             else if (fechIni < item.FechaInicio && fechFin > item.FechaInicio)
                             {
                                 ViewBag.Error = "La fecha de finalizacion solicitada para ese inmueble deberia ser menor a " + item.FechaInicio;
                             }
                             else if (fechIni > item.FechaFin && fechFin > item.FechaFin)
                             {
                                 ctosNuevos.Add(item);
                             }
                             else if (fechIni > item.FechaFin && fechFin < item.FechaFin)
                             {
                                 ViewBag.Error = "La fecha de finalizacion solicitada para ese inmueble deberia ser mayor a " + item.FechaFin;
                             }
                             else
                             {
                                 ViewBag.Error = "La fecha de inicio solicitada para ese inmueble no deberia estar entre " + item.FechaInicio + "  -  " + item.FechaFin;
                             }

                         }

                     }
                     if (count == ctosNuevos.Count)
                     {
                         c.Estado = true;
                         res = rc.Alta(c);
                         TempData["Id"] = c.IdContrato;
                         return RedirectToAction(nameof(Index));
                     }
                     else
                     {
                         ViewBag.Inmueble = ri.ObtenerTodos();
                         ViewBag.Inquilino = rinq.ObtenerTodos();
                         return View(c);
                     }

                 }
                 else

                return View(c);*/

            }
            catch (Exception ex)
            {
                ViewBag.Inquilino = rinq.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(c);
            }
        }

        // GET: ContratoController/Edit
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewBag.Inquilino = rinq.ObtenerTodos();
            ViewBag.Inmueble = ri.ObtenerTodos();
           
            var sujeto = rc.ObtenerPorId(id);
            return View(sujeto);
        }

        // POST: ContratoController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Contrato c)
        {
            try
            {
                rc.Modificacion(c);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(c);
            }
        }

        // GET: ContratoController/Delete
       [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var contrato = rc.ObtenerPorId(id);
            var lista = rpa.ObtenerTodosPorIdContrato(contrato.Id);
            var cantidadSupuesta = lista.Count;
            var fechaInicio = contrato.FechaInicio;
            var fechaFinal = contrato.FechaFin;
            TimeSpan t = fechaFinal - fechaInicio;
            var cantidadCoutas = Math.Round(t.TotalDays / 30);

            var ahora = DateTime.Now;
            if (cantidadSupuesta == cantidadCoutas && fechaFinal > ahora)
            {
                TimeSpan tp = fechaFinal - ahora;
                var meses = t.TotalDays / 30;
                var mes = (int)Math.Round(meses);
                if (mes == 1)
                {
                    var importe = lista.First().Importe;
                    ViewData["Error"] = "Si borra este contrato adquirira una multa de: $" + importe;
                }
                else if (mes >= 2)
                {
                    var importe = lista.First().Importe * 2;
                    ViewData["Error"] = "Si borra este contrato adquirira una multa de: $" + importe;
                }
                return View(contrato);
            }
            else
            {
                ViewData["Error"] = "Tiene que pagar los meses que le faltan MOROSO";
                return View(contrato);
            }


        }

        // POST: ContratoController/Delete
        [HttpPost]
       [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato c)
        {
            try
            {
                var lista = rpa.ObtenerTodosPorIdContrato(id);
                foreach (var item in lista)
                {
                    rpa.Baja(item.Id);
                }
                rc.Baja(id);

                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(c);

            }
        }
        /* // GET: ContratoController/Renovar
         * 
         public ActionResult Renovar(int id)
         {
             var c = rc.ObtenerPorId(id);

             var fechaFinal = c.FechaFin;
             var fechaAhora = DateTime.Now;
             if (fechaFinal > fechaAhora)
             {
                 TempData["Error"] = "No puede renovar un contrato que aun no ha terminado!!";
                 return RedirectToAction(nameof(Index));

             }
             var inq = rinq.ObtenerPorId(c.IdInqui);
             var i = ri.ObtenerPorId(c.IdInmu);
             ViewBag.Inquilino = inq;
             ViewBag.Inmueble = i;
             return View(c);
         }

         // POST: ContratoController/Renovar

         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Renovar(int id, Contrato c)
         {
             try
             {
                 if (ModelState.IsValid)
                 {
                     var contrato = rc.ObtenerPorId(id);
                     contrato.Estado = false;
                     rc.Modificacion(contrato);

                     c.IdInmu = contrato.IdInmu;
                     c.IdInqui = contrato.IdInqui;
                     c.Estado = true;
                     int res = rc.Alta(c);
                     TempData["Id"] = c.IdContrato;
                     return RedirectToAction(nameof(Index));
                 }
                 else
                     return View(c);
             }
             catch (Exception ex)
             {
                 ViewBag.Error = ex.Message;
                 ViewBag.StackTrate = ex.StackTrace;
                 return View(c);
             }
         }

         // GET: ContratoController/Pagar

         public ActionResult Pagar(int id)
         {
             var contrato = rc.ObtenerPorId(id);
             var listaVieja = rpa.ObtenerTodosPorIdContrato(contrato.IdContrato);
             var nroCuota = listaVieja.Count;
             var fechaInicio = contrato.FechaInicio;
             var fechaFinal = contrato.FechaFin;
             TimeSpan t = fechaFinal - fechaInicio;
             var meses = t.TotalDays / 30;
             var mes = (int)Math.Round(meses);
             var pago = new Pago();
             pago.NroPago = nroCuota + 1;
             pago.FechaPago = DateTime.Now;
             pago.IdCon = contrato.IdContrato;
             pago.Importe = contrato.Monto / mes;

             return View(pago);
         }

         // POST: ContratoController/Pagar

         public ActionResult Pagar(int id, Pago pago)
         {
             var contrato = rc.ObtenerPorId(id);
             var listaVieja = rpa.ObtenerTodosPorIdContrato(contrato.IdContrato);
             var nroCuota = listaVieja.Count;
             var fechaInicio = contrato.FechaInicio;
             var fechaFinal = contrato.FechaFin;
             TimeSpan t = fechaFinal - fechaInicio;
             var meses = t.TotalDays / 30;
             var mes = (int)Math.Round(meses);
             if (nroCuota >= mes)
             {
                 TempData["Error"] = "Ya termino de realizar los pagos";
                 return RedirectToAction("Buscar", "Contrato");
             }
             else
             {
                 pago.NroPago = nroCuota + 1;
                 pago.FechaPago = DateTime.Now;
                 pago.IdCon = contrato.IdContrato;
                 pago.Importe = contrato.Monto / mes;
                 rpa.Alta(pago);
                 TempData["Mensaje"] = "Pago realizado exitosamente!!";
                 return RedirectToAction("Buscar", "Contrato", new { id });
             }

         }
         // GET: ContratoController/Buscar

         public ActionResult Buscar(int id)
         {
             var lista = rpa.ObtenerTodosPorIdContrato(id);

             ViewBag.IdSelect = id;
             return View(lista);
         }

          // GET: ContratoController/BuscarDisponibles

         public ActionResult BuscarDisponibles()
         {
             return View();
         }

         // GET: ContratoController/MostrarDisponibles

         public ActionResult MostrarDisponibles(Contrato c)
         {
             var fechaInicio = c.FechaInicio;
             var fechaFinal = c.FechaFin;
             ViewBag.FechaInicioBusqueda = fechaInicio.Date.ToShortDateString();
             ViewBag.FechaFinalBusqueda = fechaFinal.Date.ToShortDateString();
             var lista = ri.ObtenerTodosDisponibles(fechaInicio, fechaFinal);
             return View(lista);
         }

          // GET: ContratoController/BuscarVigentes

         public ActionResult BuscarVigentes()
         {
             return View();
         }

         // GET: ContratoController/MostrarVigentes

         public ActionResult MostrarVigentes(Contrato ca)
         {
             var fechaInicio = ca.FechaInicio;
             var fechaFinal = ca.FechaFin;
             ViewBag.FechaInicioBusqueda = fechaInicio.Date.ToShortDateString();
             ViewBag.FechaFinalBusqueda = fechaFinal.Date.ToShortDateString();
             var lista = rc.ObtenerTodosVigentes(fechaInicio, fechaFinal);
             IList<Contrato> listar = new List<Contrato>();
             foreach (var item in lista)
             {
                 listar.Add(item);
             }
             return View(lista);
         }*/
    }

}

