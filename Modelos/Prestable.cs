// Clase abstracta para artículos que se pueden prestar
// Por ahora solo los libros son prestables

using System;

namespace BibliotecaApp.Modelos
{
    public abstract class Prestable : Valorable
    {
        // Días máximos de préstamo compartido por todos los prestables
        // es static para que sea un atributo de clase, no de instancia
        public static int MaxDiasPrestamo { get; set; } = 31;

        public bool Prestado { get; set; } = false;
        public DateTime? FechaPrestamo { get; set; } = null; // null si no está prestado

        // Constructor cooperativo, sube hasta Valorable -> Articulo
        protected Prestable(string titulo, int anio, DateTime fechaAdquisicion)
            : base(titulo, anio, fechaAdquisicion)
        {
        }

        // Devuelve la fecha en que hay que devolverlo
        public DateTime? FechaDevolucion()
        {
            if (FechaPrestamo == null)
                return null;
            return FechaPrestamo.Value.AddDays(MaxDiasPrestamo);
        }

        // Comprueba si está disponible para prestar
        public bool Disponible()
        {
            return !Prestado;
        }
    }
}
