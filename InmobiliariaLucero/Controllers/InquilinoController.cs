using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
    
    public class InquilinoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioInquilino rinq;

        public InquilinoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            rinq = new RepositorioInquilino(configuration);
        }

        // GET: InquilinoController/Index
        public ActionResult Index()
        {
         
            var lista = rinq.ObtenerTodos();
            return View(lista);
        }

        // GET: InquilinoController/Details
        public ActionResult Details(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
            return View(sujeto);

        }

        // GET: InquilinoController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                rinq.Alta(inquilino);
                TempData["Id"] = "creó el inquilino";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Edit
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
            
            return View(sujeto);

        }

        // POST: InquilinoController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                rinq.Modificacion(inquilino);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Delete
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
         
            return View(sujeto);

        }

        // POST: InquilinoController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inquilino inquilino)
        {
            try
            {
                rinq.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }
    }
}
