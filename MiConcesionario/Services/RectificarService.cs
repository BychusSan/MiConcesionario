using MiConcesionario.DTOs;
using MiConcesionario.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MiConcesionario.Services
{
    public class RectificarService
    {

        private readonly MiConcesionarioContext _context;

        public RectificarService(MiConcesionarioContext miConcesionarioContext)
        {
            _context = miConcesionarioContext;
        }


        //public async Task<Venta> RectificarPrecio( int idVenta, decimal descuento)
        //{
        //    var venta = await _context.Ventas.FirstOrDefaultAsync(x => x.IdVenta == idVenta);

        //    if (venta == null)
        //    {
        //     var nuevoPrecio = venta.MatriculaNavigation.Precio - (venta.MatriculaNavigation.Precio * descuento / 100);
        //    _context.Ventas.Update(venta);
        //     await _context.SaveChangesAsync();
        //    }

        //    return venta;
        //}
        public async Task<Venta> RectificarPrecio(decimal porcentajeDescuento, int idVenta)
        {
            var venta = await _context.Ventas
                .Include(v => v.MatriculaNavigation) // Asegúrate de incluir la relación necesaria
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null || venta.MatriculaNavigation == null)
            {
                // Venta o MatriculaNavigation no encontrados
                return null;
            }
            // Calcular el nuevo precio con descuento
            decimal descuento = porcentajeDescuento / 100;
            decimal nuevoPrecio = (venta.MatriculaNavigation.Precio ?? 0) - ((venta.MatriculaNavigation.Precio ?? 0) * descuento);

            // Actualizar el precio de la venta
            venta.MatriculaNavigation.Precio = nuevoPrecio;

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolver la venta actualizada
            return venta;
        }


    }
}
