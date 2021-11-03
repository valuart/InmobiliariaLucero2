using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InmobiliariaLucero.Models;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaLucero.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;

        public InquilinosController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // GET: api/Inquilinos
        [HttpGet]
        public async Task<IActionResult> Get([FromBody] Inmueble inmueble)
        {
            try
            {
                var usuario = User.Identity.Name;
                var hoy = DateTime.Now;

                var query = from cont in context.Contratos
                            join inmu in context.Inmuebles
                                on cont.Id equals inmu.Id
                            join inquilino in context.Inquilinos
                                on cont.Id equals inquilino.Id
                            where cont.FechaInicio <= hoy
                                    && cont.FechaFin >= hoy
                                    && inmu.Id == inmueble.Id
                            select inquilino;

                return Ok(query);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
