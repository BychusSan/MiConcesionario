namespace MiConcesionario.DTOs
{
    public class DTOVenta
    {
        public int IdVenta { get; set; }
        public bool Pagado { get; set; }
        public DateTime? FechaVenta { get; set; }
        public string Matricula { get; set; }
        public int ClienteId { get; set; }
    }
}
