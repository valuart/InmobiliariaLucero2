using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace InmobiliariaLucero.Controllers
{

    
    public class InmuebleController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioInmueble ri;
        private readonly RepositorioPropietario rp;
        private readonly RepositorioContrato rc;

        public InmuebleController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
            ri = new RepositorioInmueble(configuration);
            rp = new RepositorioPropietario (configuration);
            rc = new RepositorioContrato (configuration);


        }
        // GET: InmuebleController/Index
        public ActionResult Index(int id)
        {
            var lista = ri.ObtenerTodos();
            return View(lista);

        }

        // GET: InmuebleController/Details
        public ActionResult Details(int id)
        {
            Inmueble i = new Inmueble();
            i = ri.ObtenerPorId(id);
            return View(i);

        }

        // GET: InmuebleController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Propietario = rp.ObtenerTodos();
            return View();
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ri.Alta(inmueble);
                    TempData["Id"] = inmueble.Id;
                    return RedirectToAction(nameof(Index));
                    
                }
                else
                {
                    ViewBag.Propietarios = rp.ObtenerTodos();
                    return View(inmueble);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Edit
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var sujeto = ri.ObtenerPorId(id);
            ViewBag.Propietario = rp.ObtenerTodos();
            return View(sujeto);
        }

        // POST: InmuebleController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                inmueble.Id = id;
                ri.Modificacion(inmueble);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietario = rp.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Delete
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var sujeto = ri.ObtenerPorId(id);
            //ViewBag.lugar = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido + " en " + sujeto.Direccion;
            return View(sujeto);

        }

        // POST: InmuebleController/Delete
        [HttpPost]
       [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble inmueble)
        {
            try
            {
                ri.Baja(id);
                TempData["Id"] = "eliminó el inmueble";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The DELETE statement conflicted with the REFERENCE"))
                {
                    var sujeto = ri.ObtenerPorId(id);
                    ViewBag.lugar = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido + " en " + sujeto.Direccion;
                    ViewBag.Error = "No se puede eliminar el inmueble ya que tiene contratos a su nombre";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrate = ex.StackTrace;
                }
                return View(inmueble);
            }
        }
    }
}
