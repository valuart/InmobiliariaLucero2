using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaLucero.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration configuration;

        public PagosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;

        }
        // GET: api/Pagos
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Pago>>> GetPagos(int id)
        {

            try
            {
                var pagos = await _context.Pagos.Include(x => x.Contrato).Where(x =>
                     x.Id == id
                    ).ToListAsync();

                return Ok(pagos);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());

            }
        }
    }

        // PUT: api/Pagos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
}

