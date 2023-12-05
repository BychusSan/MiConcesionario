using MiConcesionario.DTOs;

namespace MiConcesionario.DTOs
{
    public class DTOClienteDetalle
    {
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public decimal? ImporteTotaldeVentas { get; set; }    // suma total de guita que se ha gastado
        public int TotalVentas { get; set; }
        public List<DTOVentaCliente>? ListaVentas { get; set; }
    }
    public class DTOVentaCliente
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Precio { get; set; }
        public string? Modelo { get; set; }
        public string? Matricula { get; set; }
        //public List<DTOActualizarPrecioVenta>? PrecioActualizado { get; set; }

    }
    public class DTOVentaAgrupada
    {
        public int IdCliente { get; set; }
        public string? NombreCliente { get; set; }
        public int CantidadCochesVendidos { get; set; }
        public List<DTOVentaCliente>? Venta { get; set; }
    }

    //public class DTOActualizarPrecioVenta
    //{
    //    public int IdVenta { get; set; }
    //    public int IdCliente { get; set; }
    //    public decimal PorcentajeDescuento { get; set; }

    //    public decimal? Precio { get; set; }

    //}


}

