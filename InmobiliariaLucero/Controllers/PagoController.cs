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

    
    public class PagoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPago rpa;
        private readonly RepositorioContrato rc;

        public PagoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            rc = new RepositorioContrato(configuration);
            rpa = new RepositorioPago(configuration);
        }
        // GET: PagoController/Index
        public ActionResult Index()
        {
            var lista = rpa.ObtenerTodos();
            return View(lista);
        }

        // GET: PagoController/Details
        public ActionResult Details(int id)
        {
            var sujeto = rpa.ObtenerPorId(id);
            return View(sujeto);
        }

        // GET: PagoController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Contrato = rc.ObtenerTodos();
            return View();
        }
    

        // POST: PagoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Pago pa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rpa.Alta(pa);
                    TempData["Id"] = pa.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Contrato = rc.ObtenerTodos();
                    return View(pa);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(pa);
            }
        }

        // GET: PagoController/Edit
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewBag.Contrato = rc.ObtenerTodos();
            var sujeto = rpa.ObtenerPorId(id);
          
            return View(sujeto);
        }

        // POST: PagoController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Pago pa)
        {
            try
            {
               
                rpa.Modificacion(pa);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Contrato = rc.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(pa);
            }
        }

        // GET: PagoController/Delete
       [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var sujeto = rpa.ObtenerPorId(id);
           
            return View(sujeto);
        }

        // POST: PagoController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Pago pa)
        {
            try
            {
                rpa.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(pa);
            }
        }
    }
}
