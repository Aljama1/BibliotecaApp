// Clase Audiolibro: hereda de Valorable -> Articulo
// No se presta, se descarga si está dentro del periodo de disponibilidad

using System;

namespace BibliotecaApp.Modelos
{
    public class Audiolibro : Valorable
    {
        public string Autor { get; set; } = "";
        public DateTime FechaInicioDisponibilidad { get; set; }
        public DateTime FechaFinDisponibilidad { get; set; }

        // Constructor cooperativo
        public Audiolibro(string titulo, int anio, DateTime fechaAdquisicion,
                          DateTime fechaInicio, DateTime fechaFin, string autor = "")
            : base(titulo, anio, fechaAdquisicion)
        {
            FechaInicioDisponibilidad = fechaInicio;
            FechaFinDisponibilidad = fechaFin;
            Autor = autor;
        }

        // Comprueba si se puede descargar ahora mismo
        public bool EstaDisponible()
        {
            DateTime hoy = DateTime.Today;
            return hoy >= FechaInicioDisponibilidad && hoy <= FechaFinDisponibilidad;
        }

        public override string ToString()
        {
            string disponible = EstaDisponible() ? "Disponible" : "No disponible";
            return $"[Audiolibro] {Titulo} ({Anio}) - {disponible}";
        }
    }
}
