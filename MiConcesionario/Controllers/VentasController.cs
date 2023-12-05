using MiConcesionario.DTOs;
using MiConcesionario.Models;
using MiConcesionario.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiConcesionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class VentasController : ControllerBase
    {

        private readonly MiConcesionarioContext _context;
        private readonly ILogger<VentasController> _logger;
        private readonly RectificarService _rectificarService;


        public VentasController(MiConcesionarioContext context, ILogger<VentasController> logger, RectificarService rectificarService)
        {
            _context = context;
            _logger = logger;
            _rectificarService = rectificarService;
        }

        #region GET

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            _logger.LogInformation("OBTENIENDO VENTAS"); // Ejecutar y ver el resultado en consola o en ventana salida
            var editoriales = await _context.Ventas.ToListAsync();
            if (editoriales == null)
            {
                return NotFound();
            }

            return Ok(editoriales);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Venta>> GetEditorialesId2([FromRoute] int id)
        {

            var ventas = await _context.Ventas.FindAsync(id);

            if (ventas == null)
            {
                return NotFound("No existe esa venta.");
            }

            return Ok(ventas);
        }


        [HttpGet("ObtenerVentas/Pagadas")]
        public async Task<ActionResult<List<Venta>>> ObtenerVentasPagadas(bool pagadas = false)
        {
            var ventas = await _context.Ventas.Where(f => f.Pagado == pagadas).ToListAsync();

            return ventas;
        }

        [HttpGet("ObtenerVentaPorIdCliente/{IdCliente}")]

        public async Task<ActionResult<List<Venta>>> ObtenerFacturasDeCliente(int IdCliente)
        {
            var ventas = await _context.Ventas
                .Where(f => f.ClienteId == IdCliente)
                .ToListAsync();

            return ventas;
        }


        #endregion

        #region POST

        [HttpPost]
        public async Task<ActionResult<Venta>> PostVentas(DTOVenta ventas)
        {

            var clienteExiste = await _context.Ventas.FindAsync(ventas.ClienteId);
            if (clienteExiste == null)
            {
                return BadRequest("El cliente no existe.");
            }

            var cocheExiste = await _context.Coches.FindAsync(ventas.Matricula);
            if (cocheExiste == null)
            {
                return BadRequest("La matrícula del coche no existe.");
            }

            var newVenta = new Venta()
            {
                IdVenta = ventas.IdVenta,
                Pagado = ventas.Pagado,
                Matricula = ventas.Matricula,
                ClienteId = ventas.ClienteId
            };

            await _context.AddAsync(newVenta);
            await _context.SaveChangesAsync();

            return Created("Ventas", new { Venta = newVenta.IdVenta });
        }

        #endregion

        #region PUT

        [HttpPut]
        public async Task<ActionResult<Venta>> PutVentas(DTOVenta dtoventa)
        {

            var ventaExiste = await _context.Ventas.AsTracking().FirstOrDefaultAsync(x => x.IdVenta == dtoventa.IdVenta);

            if (ventaExiste == null)
            {
                return NotFound("Esa VENTA no existe.");
            }


            var clienteExiste = await _context.Clientes.FindAsync(dtoventa.ClienteId);
            if (clienteExiste == null)
            {
                return BadRequest("El cliente no existe.");
            }

            var matriculaExiste = await _context.Coches.AnyAsync(x => x.Matricula == dtoventa.Matricula);
            if (!matriculaExiste)
            {
                return BadRequest("La matricula no coincide con ningún coche existente.");
            }


            ventaExiste.IdVenta = dtoventa.IdVenta;
            ventaExiste.Pagado = dtoventa.Pagado;
            ventaExiste.Matricula = dtoventa.Matricula;
            ventaExiste.ClienteId = dtoventa.ClienteId;


            _context.Update(ventaExiste);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("rectificar-precio/{idVenta}")]
        public async Task<ActionResult<Venta>> RectificarPrecio(decimal descuento, int idVenta)
        {
            var venta = await _rectificarService.RectificarPrecio(descuento, idVenta);
            if (venta == null)
            {
                return NotFound();
            }
            return Ok(venta);
        }


        #endregion

        #region DELETE

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> BorrarVenta(int id)
        {

            var venta = await _context.Ventas.FirstOrDefaultAsync(x => x.IdVenta == id);

            if (venta is null)
            {
                return NotFound("La VENTA no existe");
            }

            _context.Remove(venta);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion

    }
}
