using System;
using System.Collections.Generic;

namespace MiConcesionario.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public bool? Pagado { get; set; }

    public DateOnly? FechaVenta { get; set; }

    public string? Matricula { get; set; }

    public int? ClienteId { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Coch? MatriculaNavigation { get; set; }
}
