// Clase abstracta para artículos que se pueden valorar
// Libros y audiolibros son valorables

using System.Collections.Generic;
using System.Linq;

namespace BibliotecaApp.Modelos
{
    public abstract class Valorable : Articulo
    {
        // Lista de valoraciones que tiene este artículo
        public List<Valoracion> Valoraciones { get; set; } = new();

        // Constructor cooperativo, pasa los parámetros al padre (Articulo)
        protected Valorable(string titulo, int anio, System.DateTime fechaAdquisicion)
            : base(titulo, anio, fechaAdquisicion)
        {
        }

        // Calcula la media de las valoraciones automáticamente
        public double ValoracionMedia()
        {
            if (Valoraciones.Count == 0)
                return 0;
            return Valoraciones.Average(v => v.Puntuacion);
        }

        // Añade una nueva valoracion a la lista
        public void AnadirValoracion(Valoracion v)
        {
            Valoraciones.Add(v);
        }
    }
}
