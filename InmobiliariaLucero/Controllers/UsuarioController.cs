using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;



namespace InmobiliariaLucero.Controllers
{
    
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment envir;
        private readonly RepositorioUsuario ru;

        public UsuarioController(IConfiguration configuration, IWebHostEnvironment envir)
        {
            this.configuration = configuration;
            ru = new RepositorioUsuario(configuration);
            this.envir = envir;
        }

        // GET: UsuarioController/Index
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Index()
        {
            var lista = ru.ObtenerTodos();
            return View(lista);
        }

        // GET: UsuarioController/Details
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Details(int id)
        {
            var e = ru.ObtenerPorId(id);
            return View(e);
        }

        // GET: UsuarioController/Create
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Create()
        {

            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = ru.Alta(u);
                TempData["Id"] = u.Id;
                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = envir.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    ru.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(u);
            }
        }

        // GET: Admin/Edit
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var sujeto = ru.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(sujeto);
        }

        // POST: Admin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            try
            {
                 var usuario = ru.ObtenerPorId(id);
                 if (User.IsInRole("Usuario"))
                 {
                     vista = "Perfil";
                     var usuarioActual = ru.ObtenerPorEmail(User.Identity.Name);
                     if (usuarioActual.Id != id)
                     {
                         return RedirectToAction(nameof(Index), "Home");
                     }
                     else
                     {
                        ru.Modificacion(u);
                        TempData["Mensaje"] = "Datos guardados correctamente";
                        return RedirectToAction(nameof(Index));
                    }

               }
                // TODO: Add update logic here
                u.Clave = usuario.Clave;
                u.Avatar = usuario.Avatar;
                ru.Modificacion(u);
                TempData["Mensaje"] = "Datos guardados correctamente";

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View( vista, u);
            }
        }

        // GET: Admin/Delete
            [Authorize(Policy = "SuperAdministrador")] 
            public ActionResult Delete(int id)
            {
                var u = ru.ObtenerPorId(id);
                return View(u);
            }

            // POST: Admin/Delete
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Policy = "SuperAdministrador")]
            public ActionResult Delete(int id, Usuario u)
            {
                try
                {
                    int res = ru.Baja(id);
                TempData["Mensaje"] = "Usuario Eliminado";
                return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

        // GET: UsuarioController/Perfil  
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = ru.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }
        // POST: UsuarioController/Perfil/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Perfil(Usuario user)
        {
            try
            {
                //var u = ru.ObtenerPorEmail(User.Identity.Name);
                //u.AvatarFile = user.AvatarFile;
                //if (u.AvatarFile != null && u.Id > 0)
                //{
                //    string wwwPath = envir.WebRootPath;
                //    string path = Path.Combine(wwwPath, "Uploads");
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                //    string pathCompleto = Path.Combine(path, fileName);
                //    u.Avatar = Path.Combine("/Uploads", fileName);
                //    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                //    {
                //        u.AvatarFile.CopyTo(stream);
                //    }
                //    ru.Modificacion(u);
                //}
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }


        //  GET: UsuarioController/Avatar
        [Authorize]
        public IActionResult Avatar()
        {
            var u = ru.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = envir.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        // GET: UsuarioController/Foto
         [Authorize]
        public ActionResult Foto()
        {
            try
            {
                var u = ru.ObtenerPorEmail(User.Identity.Name);
                var stream = System.IO.File.Open(
                    Path.Combine(envir.WebRootPath, u.Avatar.Substring(1)),
                    FileMode.Open,
                    FileAccess.Read);
                var ext = Path.GetExtension(u.Avatar);
                return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
            }
            catch  //(Exception ex)
            {
                throw;
            }
        }
        // GET: UsuarioController/Datos
        [Authorize]
        public ActionResult Datos()
        {
            try
            {
                var u = ru.ObtenerPorEmail(User.Identity.Name);
                string buffer = "Nombre;Apellido;Email" + Environment.NewLine +
                    $"{u.Nombre};{u.Apellido};{u.Email}" ;
                var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
                var res = new FileStreamResult(stream, "text/plain");
                res.FileDownloadName = "Datos.csv";
                return res;
            }
            catch //(Exception ex)
            {
                throw;
              
            }
        }


        // GET: UsuarioController/Login/
        [AllowAnonymous] 
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        // POST: UsuarioController/Login/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = ru.ObtenerPorEmail(login.Email);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                       TempData["returnUrl"] = returnUrl;
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: UsuarioController/salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}