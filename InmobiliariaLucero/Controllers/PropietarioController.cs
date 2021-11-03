using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
    
    public class PropietarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPropietario rp;
        private readonly RepositorioInmueble ri;

        public PropietarioController(IConfiguration configuration)
        {
            ri = new RepositorioInmueble(configuration);
            rp = new RepositorioPropietario(configuration);
            this.configuration = configuration;
        }


        // GET: PropietarioController/Index
        public ActionResult Index(int id)
        {
          
            var lista = rp.ObtenerTodos();
            return View(lista);
        }

        // GET: PropietarioController/Details
        public ActionResult Details(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
            return View(sujeto);

        }

        // GET: PropietarioController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(Propietario p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Reemplazo de clave plana por clave con hash
                        p.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: p.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    int res = rp.Alta(p);
                    TempData["Id"] = p.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(p);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: PropietarioController/Edit
        [Authorize]
        public ActionResult Edit(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
            return View(sujeto);
        }

        // POST: PropietarioController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Propietario p)
        {
            try
            {
                rp.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: PropietarioController/Delete
        [Authorize]
        public ActionResult Delete(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
          
            return View(sujeto);
        }

        // POST: PropietarioController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id, Propietario p)
        {
            try
            {
            
                rp.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }
    }
}
