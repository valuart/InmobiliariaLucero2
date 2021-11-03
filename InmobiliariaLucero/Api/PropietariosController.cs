using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase//
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PropietariosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }
        // GET: api/<PropietariosController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                /*contexto.Inmuebles
                    .Include(x => x.Duenio)
                    .Where(x => x.Duenio.Nombre == "")//.ToList() => lista de inmuebles
                    .Select(x => x.Duenio)
                    .ToList();//lista de propietarios
                var usuario = User.Identity.Name;
                var res = contexto.Propietario.Select(x => new { x.Nombre, x.Apellido, x.Email }).SingleOrDefault(x => x.Email == usuario);
                return Ok(res); */
                // return contexto.Propietario;

                var usuario = User.Identity.Name;
                var propietario = contexto.Propietarios.FirstOrDefault(x => x.Email == usuario);



                return Ok(propietario);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        // GET api/<PropietariosController>/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await contexto.Propietarios.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<PropietariosController>/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] Login login)
        {

            try
            {


                string hashed1 = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                   password: login.Clave,
                   salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                   prf: KeyDerivationPrf.HMACSHA1,
                   iterationCount: 1000,
                   numBytesRequested: 256 / 8));
                var p = contexto.Propietarios.FirstOrDefault(x => x.Email == login.Email);
                string hashed2 = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                   password: login.Clave,
                   salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                   prf: KeyDerivationPrf.HMACSHA1,
                   iterationCount: 1000,
                   numBytesRequested: 256 / 8));
                if (p == null || hashed1 != hashed2)
                {
                    return BadRequest("Email y/o Contraseña incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim("FullName", p.Nombre + " " + p.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // POST api/<PropietariosController>/login
        [HttpGet("propietarioActual")]
        //public async Task<IActionResult> PropietarioActual([FromBody] LoginView loginView)
        public async Task<IActionResult> PropietarioActual()
        {
            try
            {
                return Ok(
                    contexto.Propietarios

                    .Select(x => new { x.id, x.Nombre, x.Apellido, x.Dni, x.Email, x.Clave, x.Telefono })
                    .FirstOrDefault(x => x.Email == User.Identity.Name));

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // POST api/<PropietariosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Propietario entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await contexto.Propietarios.AddAsync(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.id }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<PropietariosController>/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Propietario entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.id = id;
                    contexto.Propietarios.Update(entidad);
                    await contexto.SaveChangesAsync();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<PropietariosController>/borra/4
        [HttpDelete("borra{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var p = contexto.Propietarios.Find(id);
                    if (p == null)
                        return NotFound();
                    contexto.Propietarios.Remove(p);
                    contexto.SaveChanges();
                    return Ok(p);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Propietarios/test
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            try
            {
                return Ok("anduvo");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Propietarios/test/5
        [HttpGet("test/{codigo}")]
        [AllowAnonymous]
        public IActionResult Code(int codigo)
        {
            try
            {
                //StatusCodes.Status418ImATeapot //constantes con códigos
                return StatusCode(codigo, new { Mensaje = "Anduvo", Error = false });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
