using System;
using System.Collections.Generic;

namespace MiConcesionario.Models;

public partial class Coch
{
    public string Matricula { get; set; } = null!;

    public string? Modelo { get; set; }

    public decimal? Precio { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
