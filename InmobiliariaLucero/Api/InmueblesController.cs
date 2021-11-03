using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InmobiliariaLucero.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

using System.IO;

namespace InmobiliariaLucero.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment envir;

        public InmueblesController(DataContext context, IConfiguration configuration, IWebHostEnvironment envir)
        {
            this.context = context;
            this.configuration = configuration;
            this.envir = envir;


        }

        // GET: api/Inmuebles                               
        [HttpGet]
        public async Task<IActionResult> ListaInmuebles()
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(context.Inmuebles.Include(x => x.Propietario)
                    .Where(x => x.Propietario.Email == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Inmuebles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inmueble>> GetInmueble(int id)
        {

            try
            {
                var usuario = User.Identity.Name;
                return Ok(context.Inmuebles.Include(x => x.Propietario)
                    .Where(x => x.Propietario.Email == usuario)
                    .Single(x => x.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET: api/Inmuebles/5
        [HttpGet("InmueblesAlquilados")]
        public async Task<IActionResult> InmueblesAlquilados()
        {
            try
            {
                var usuario = User.Identity.Name;
                var hoy = DateTime.Now;

                var query = from inmu in context.Inmuebles
                            join cont in context.Contratos
                                on inmu.Id equals cont.Id
                            where cont.FechaInicio <= hoy && cont.FechaFin >= hoy && usuario == inmu.Propietario.Email
                            select cont;

               return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Inmuebles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEstado(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var i = context.Inmuebles.Include(x => x)
                    .FirstOrDefault(x => x.Id == id && x.Propietario.Email == usuario);
                if (i != null)
                {
                    i.Estado = !i.Estado;
                    context.Inmuebles.Update(i);
                    await context.SaveChangesAsync();
                    return Ok(i);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        // POST api/<controller>
        //este metodo envia la foto del inmueble
      [HttpPost]
       public async Task<IActionResult> Post([FromBody] Inmueble inmueble)
       {
         try
            {
              var usuario = User.Identity.Name;
                    if (inmueble.ImagenFile != null)
                    {
                        var stream = new MemoryStream(Convert.FromBase64String(inmueble.ImagenFile));
                        IFormFile imagen = new FormFile(stream, 0, stream.Length, "inmueble", ".jpg");
                        string wwwPath = envir.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = "imagen_" + inmueble.Direccion + Path.GetExtension(imagen.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        inmueble.Imagen = Path.Combine("/Uploads", fileName);
                        using (FileStream streamF = new FileStream(pathCompleto, FileMode.Create))
                        {
                            imagen.CopyTo(streamF);
                        }
                        inmueble.IdPropie = context.Propietarios.Single(x => x.Email == usuario).Id;
                        context.Inmuebles.Add(inmueble);
                        await context.SaveChangesAsync();
                        return Ok(inmueble);
                        //return CreatedAtAction(nameof(Get), new {id = inmueble.Id}, inmueble);
                    }
                    else
                    {
                        return BadRequest("No entra al if");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }
 }    

