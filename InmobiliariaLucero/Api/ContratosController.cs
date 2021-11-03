using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaLucero.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContratosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;

        public ContratosController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;

        }
 
        // GET: api/Contratos
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var lista = await context.Contratos
                                .Include(x => x.Inquilino)
                                .Include(x => x.Inmueble)
                                .Where(x => x.Inmueble.Propietario.Email == usuario).ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var contrato = await context.Contratos.Include(x => x.Inquilino)
                                    .Include(x => x.Inmueble)
                                    .Where(x => x.Inmueble.Propietario.Email == usuario)
                                    .SingleOrDefaultAsync(x => x.Id == id);
                return contrato != null ? Ok(contrato) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Contratoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    }
}
