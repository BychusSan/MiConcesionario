using MiConcesionario.DTOs;
using MiConcesionario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiConcesionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CochesController : ControllerBase
    {

        private readonly MiConcesionarioContext _context;
        private readonly ILogger<CochesController> _logger;

        public CochesController(MiConcesionarioContext context, ILogger<CochesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coch>>> GetCoches()
        {
            _logger.LogInformation("OBTENIENDO CLIENTES"); // Ejecutar y ver el resultado en consola o en ventana salida
            var editoriales = await _context.Coches.ToListAsync();
            if (editoriales == null)
            {
                return NotFound();
            }

            return Ok(editoriales);

        }



        [HttpGet("parametrocontienequerystring")]
        public async Task<ActionResult<IEnumerable<Coch>>> GetCochesEntrePreciosQueryString([FromQuery] decimal PrecioMinimo, [FromQuery] decimal PrecioMaximo)
        {
            var cochecitos = await _context.Coches.Where(x => x.Precio > PrecioMinimo && x.Precio < PrecioMaximo).ToListAsync();
            return Ok(cochecitos);
        }

        //[HttpGet("ObtenerCoches/NoVendidos")]
        //public async Task<ActionResult<List<DTOCoche>>> CochesNoVendidos()
        //{
        //    var cochesConVenta = await _context.Ventas.Select(v => v.Matricula).ToListAsync();

        //    var cochesNoVendidos = await _context.Coches.Where(x => !cochesConVenta.Contains(x.Matricula))
        //    .Select(x => new DTOCoche {Matricula = x.Matricula, Modelo = x.Modelo}).ToListAsync();

        //    return cochesNoVendidos;
        //}

        [HttpGet("CochesNoVendidos")]
        public async Task<ActionResult<List<Coch>>> ObtenerCochesNoVendidos()

        {

            var cochesNoVendidos = await _context.Coches.Where(coche => !_context.Ventas.Any(venta => venta.Matricula == coche.Matricula)).ToListAsync();

            if (cochesNoVendidos == null || cochesNoVendidos.Count == 0)
            {
                return NotFound("No se encontraron coches que no se han vendido.");
            }

            return Ok(cochesNoVendidos);

        }



        #endregion

        #region POST

        [HttpPost]
        public async Task<ActionResult<Coch>> PostCoches(DTOCoche coches)
        {
            var newCoche = new Coch()
            {
                Matricula = coches.Matricula,
                Modelo = coches.Modelo,
                Precio = coches.Precio,
            };

            await _context.AddAsync(newCoche);
            await _context.SaveChangesAsync();

            return Created("Coches", new { Matricula = newCoche.Matricula });
        }

        #endregion

        #region PUT
        // 2 OPCIONES DE PUT, LA SEGUNDA ME GUSTA MÁS

        [HttpPut]
        public async Task<ActionResult<Coch>> PutCoche(DTOCoche dtocochesput)
        {
            var cocheExiste = await _context.Coches.AsTracking().FirstOrDefaultAsync(x => x.Matricula == dtocochesput.Matricula);

            if (cocheExiste == null)
            {
                return NotFound("La MATRICULA no coincide.");
            }

            cocheExiste.Matricula = dtocochesput.Matricula;
            cocheExiste.Modelo = dtocochesput.Modelo;
            cocheExiste.Precio = dtocochesput.Precio;

            _context.Update(cocheExiste);

            await _context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPut("PRUEBAMODIFICACION/{matricula}")]
        public async Task<ActionResult<Coch>> PutCoches(string matricula, DTOCoche coches)
        {
            var prueba = await _context.Coches.FirstOrDefaultAsync(c => c.Matricula == matricula);

            if (prueba == null)
            {
                return NotFound("No existe la matricula");
            }

            prueba.Modelo = coches.Modelo;
            prueba.Precio = coches.Precio;
            prueba.Matricula = coches.Matricula; 

            _context.Update(prueba);
            await _context.SaveChangesAsync();

            return Ok(prueba);
        }



        #endregion

        #region DELETE

        [HttpDelete("{MATRICULA}")]
        public async Task<ActionResult> BorrarLibro(string MATRICULA)
        {

            var coche = await _context.Coches.Include(x => x.Venta).FirstOrDefaultAsync(x => x.Matricula == MATRICULA);

            if (coche is null)
            {
                return NotFound("La matricula no existe");
            }
            if (coche.Venta.Any())
            {
                return BadRequest("No se puede eliminar. El coche tiene ventas asociadas.");
            }

            _context.Remove(coche);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion


    }
}
