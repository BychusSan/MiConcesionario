using System;
using System.Collections.Generic;

namespace MiConcesionario.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
