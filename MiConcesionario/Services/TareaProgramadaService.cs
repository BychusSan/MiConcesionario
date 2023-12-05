using Microsoft.EntityFrameworkCore;
using MiConcesionario.Models;

namespace MiConcesionario.Services
{
    public class TareaProgramadaService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly string nombreArchivo = "Archivo.txt";
        private Timer timer;

        public TareaProgramadaService(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
           _serviceProvider = serviceProvider;
           _env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(EscribirDatos, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Escribir("Proceso iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Escribir("Proceso finalizado");
            return Task.CompletedTask; // Parar la depuración desde el icono de IIS para que se ejecute el StopAsync
        }

        private async void EscribirDatos(object state)
        {

            if (DateTime.Now.Hour == 23)
            {


                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MiConcesionarioContext>();
                    var cochesNoVendidos = await context.Coches.AsTracking().Where(x => x.Venta.Count == 0).ToListAsync();

                    foreach (var coche in cochesNoVendidos)
                    {
                        coche.Precio += 500;
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

        private void Escribir(string mensaje)
        {
            var ruta = $@"{_env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {mensaje}");
            }
        }
    }
}
