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
    public class ClientesController : ControllerBase
    {

        private readonly MiConcesionarioContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(MiConcesionarioContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            _logger.LogInformation("OBTENIENDO CLIENTES"); // Ejecutar y ver el resultado en consola o en ventana salida
            var editoriales = await _context.Clientes.ToListAsync();
            if (editoriales == null)
            {
                return NotFound();
            }

            return Ok(editoriales);
        }


        [HttpGet("PROBAR-EXCEPTION")]
        public async Task<List<Cliente>> GetClientes2()
        {

            var lista = await _context.Clientes.ToListAsync();
            throw new Exception("Error deliberado");

            return lista;
        }


        [HttpGet("SumaDeVentasPorCliente")]
        public async Task<ActionResult<List<DTOClienteDetalle>>> ObtenerSumaDeVentasPorCliente()
        {
            var clientesConDetalles = await _context.Clientes
                .Include(c => c.Venta)
                .Select(c => new DTOClienteDetalle
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    ImporteTotaldeVentas = c.Venta.Sum(c => c.MatriculaNavigation.Precio),
                    TotalVentas = c.Venta.Count(),
                    ListaVentas = c.Venta.Select(v => new DTOVentaCliente

                    {
                        IdVenta = v.IdVenta,
                        Precio = v.MatriculaNavigation.Precio,
                        Modelo = v.MatriculaNavigation.Modelo,
                        Matricula = v.Matricula,
                    }).ToList()
                })
                .ToListAsync();

            return Ok(clientesConDetalles);
        }

        [HttpGet("ventas-agrupadas")]
        public async Task<ActionResult<List<DTOVentaAgrupada>>> ObtenerVentasAgrupadasPorCliente()
        {
            var ventasAgrupadas = await _context.Ventas
                .GroupBy(v => new { v.Cliente.IdCliente, v.Cliente.Nombre })
                .Select(g => new DTOVentaAgrupada
                {
                    IdCliente = g.Key.IdCliente,
                    NombreCliente = g.Key.Nombre,
                    CantidadCochesVendidos = g.Count(),
                    Venta = g.Select(v => new DTOVentaCliente
                    {
                        IdVenta = v.IdVenta,
                        Precio = v.MatriculaNavigation.Precio,
                        Modelo = v.MatriculaNavigation.Modelo,
                        Matricula = v.Matricula
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ventasAgrupadas);
        }


        #endregion

        #region POST

        [HttpPost]

        public async Task<ActionResult<Cliente>> PostCoches(DTOCliente clientes)
        {
            var newCliente = new Cliente()
            {
                Nombre = clientes.Nombre,
            };

            await _context.AddAsync(newCliente);
            await _context.SaveChangesAsync();

            return Created("Clientes", new { ID = newCliente.IdCliente });
        }

        #endregion

        #region PUT
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutClientes([FromRoute] int id, [FromBody] DTOCliente dtoclientesput)
        {
            var clienteExiste = await _context.Clientes.AsTracking().FirstOrDefaultAsync(x => x.IdCliente == id);

            if (clienteExiste == null)
            {
                return NotFound("El ID cliente no coincide.");
            }

            clienteExiste.Nombre = dtoclientesput.Nombre;

            _context.Update(clienteExiste);

            await _context.SaveChangesAsync();
            return NoContent();

        }



        #endregion

        #region DELETE

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> BorrarCliente(int id)
        {

            var ventaExiste = await _context.Clientes.Include(x => x.Venta).FirstOrDefaultAsync(x => x.IdCliente == id);
            if (ventaExiste is null)
            {
                return NotFound("El cliente no existe");
            }

            if (ventaExiste.Venta.Any())
            {
                return BadRequest("No se puede eliminar. El cliente tiene ventas asociadas.");
            }

            _context.Remove(ventaExiste);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion


    }
}
