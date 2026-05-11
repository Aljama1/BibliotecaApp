// Clase que representa una valoracion de un artículo
// Tiene puntuacion obligatoria y comentario opcional

namespace BibliotecaApp.Modelos
{
    public class Valoracion
    {
        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public double Puntuacion { get; set; }       // obligatoria, entre 0 y 10
        public string Comentario { get; set; } = "";  // opcional
        public string PalabrasClave { get; set; } = ""; // opcionales, separadas por comas
        public string IdUsuario { get; set; } = "";   // quien hace la valoracion

        public Valoracion(int articuloId, double puntuacion, string idUsuario,
                          string comentario = "", string palabrasClave = "")
        {
            ArticuloId = articuloId;
            Puntuacion = puntuacion;
            IdUsuario = idUsuario;
            Comentario = comentario;
            PalabrasClave = palabrasClave;
        }
    }
}
