// Clase base abstracta para todos los artículos de la biblioteca
// Aquí pongo lo que comparten libros y audiolibros

using System;

namespace BibliotecaApp.Modelos
{
    // Esto es una clase abstracta, no se puede instanciar directamente
    public abstract class Articulo
    {
        // Propiedades comunes a todos los artículos
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public int Anio { get; set; }
        public DateTime FechaAdquisicion { get; set; }

        // Constructor cooperativo, llama al de Object implícitamente
        protected Articulo(string titulo, int anio, DateTime fechaAdquisicion)
        {
            Titulo = titulo;
            Anio = anio;
            FechaAdquisicion = fechaAdquisicion;
        }

        // Para mostrar en la lista, cada subclase lo puede sobreescribir
        public override string ToString()
        {
            return $"[{Anio}] {Titulo}";
        }
    }
}
