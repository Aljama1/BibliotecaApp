// Clase Libro: hereda de Prestable (que hereda de Valorable -> Articulo)
// Los libros tienen ISBN-10, se prestan y se valoran

using System;

namespace BibliotecaApp.Modelos
{
    public class Libro : Prestable
    {
        public string Isbn { get; set; } = "";
        public string Autor { get; set; } = "";

        // Constructor cooperativo, sube la cadena de herencia
        public Libro(string titulo, int anio, DateTime fechaAdquisicion, string isbn, string autor = "")
            : base(titulo, anio, fechaAdquisicion)
        {
            Isbn = isbn;
            Autor = autor;
        }

        public override string ToString()
        {
            string estado = Prestado ? "Prestado" : "Disponible";
            return $"[Libro] {Titulo} ({Anio}) - ISBN: {Isbn} - {estado}";
        }
    }
}
