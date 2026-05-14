// Controlador principal de la biblioteca
// Aquí va la lógica de negocio: validaciones, búsquedas, préstamos, etc.
// El controlador conecta la vista con los modelos y la BD

using System;
using System.Collections.Generic;
using System.Linq;
using BibliotecaApp.Datos;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Controlador
{
    public class ControladorBiblioteca
    {
        private readonly GestorBD _bd;

        // Listas en memoria, se cargan al arrancar desde la BD
        public List<Libro> Libros { get; private set; } = new();
        public List<Audiolibro> Audiolibros { get; private set; } = new();

        public ControladorBiblioteca(string rutaBD)
        {
            _bd = new GestorBD(rutaBD);

            // Inicializar la BD al arrancar (crea tablas si no existen)
            using var con = _bd.CrearConexionBD();
            _bd.InicializarBD(con);

            // Cargar todo desde la BD a memoria
            CargarDatos();
        }

        // Carga libros y audiolibros desde la BD a las listas en memoria
        private void CargarDatos()
        {
            Libros = _bd.ObtenerLibros();
            Audiolibros = _bd.ObtenerAudiolibros();

            // Cargar también las valoraciones para cada artículo
            foreach (var libro in Libros)
                libro.Valoraciones = _bd.ObtenerValoraciones(libro.Id, "libro");
            foreach (var audio in Audiolibros)
                audio.Valoraciones = _bd.ObtenerValoraciones(audio.Id, "audiolibro");
        }

        // ---- VALIDACIONES ----

        // Valida el ISBN-10: suma de dígito*peso (10,9,...,1) debe ser múltiplo de 11
        public static bool ValidarIsbn(string isbn)
        {
            if (isbn.Length != 10) return false;

            int suma = 0;
            for (int i = 0; i < 10; i++)
            {
                if (!char.IsDigit(isbn[i])) return false;
                suma += (isbn[i] - '0') * (10 - i);
            }
            return suma % 11 == 0;
        }

        // Pone la primera letra en mayúscula y quita espacios al principio y al final
        public static string FormatearTitulo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";
            texto = texto.Trim();
            return char.ToUpper(texto[0]) + texto[1..];
        }

        // Comprueba que el año esté entre 1500 y el año actual
        private static bool AnioValido(int anio)
        {
            return anio >= 1500 && anio <= DateTime.Now.Year;
        }

        // ---- BUSCAR ITEMS ----
        // Devuelve artículos que cumplan los filtros indicados
        // titulos: busca por título (contiene el texto)
        // filtros: diccionario con claves como "autor", "anio", "tipo", "disponible"
        public List<Articulo> BuscarItems(string[] titulos = null!, Dictionary<string, object> filtros = null!)
        {
            var resultado = new List<Articulo>();

            // Añadimos todos los artículos al resultado inicial
            resultado.AddRange(Libros);
            resultado.AddRange(Audiolibros);

            // Filtrar por título (si se pasaron títulos a buscar)
            if (titulos != null && titulos.Length > 0)
            {
                resultado = resultado.Where(a =>
                    titulos.Any(t => a.Titulo.Contains(t, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Aplicar filtros extra
            if (filtros != null)
            {
                foreach (var filtro in filtros)
                {
                    switch (filtro.Key.ToLower())
                    {
                        case "tipo":
                            string tipo = filtro.Value.ToString()!.ToLower();
                            if (tipo == "libro")
                                resultado = resultado.OfType<Libro>().Cast<Articulo>().ToList();
                            else if (tipo == "audiolibro")
                                resultado = resultado.OfType<Audiolibro>().Cast<Articulo>().ToList();
                            break;

                        case "anio":
                            int anio = (int)filtro.Value;
                            resultado = resultado.Where(a => a.Anio == anio).ToList();
                            break;

                        case "autor":
                            string autor = filtro.Value.ToString()!;
                            resultado = resultado.Where(a =>
                            {
                                if (a is Libro l) return l.Autor.Contains(autor, StringComparison.OrdinalIgnoreCase);
                                if (a is Audiolibro au) return au.Autor.Contains(autor, StringComparison.OrdinalIgnoreCase);
                                return false;
                            }).ToList();
                            break;

                        case "disponible":
                            bool disp = (bool)filtro.Value;
                            resultado = resultado.Where(a =>
                            {
                                if (a is Libro lb) return lb.Disponible() == disp;
                                if (a is Audiolibro au) return au.EstaDisponible() == disp;
                                return false;
                            }).ToList();
                            break;
                    }
                }
            }

            return resultado;
        }

        // ---- CRUD LIBROS ----

        public (bool ok, string error) AgregarLibro(Libro libro)
        {
            // Validar ISBN
            if (!ValidarIsbn(libro.Isbn))
                return (false, "El ISBN-10 no es válido.");

            // Validar año
            if (!AnioValido(libro.Anio))
                return (false, $"El año debe estar entre 1500 y {DateTime.Now.Year}.");

            // Formatear título
            libro.Titulo = FormatearTitulo(libro.Titulo);

            // Comprobar que el ISBN no está duplicado
            if (Libros.Any(l => l.Isbn == libro.Isbn))
                return (false, "Ya existe un libro con ese ISBN.");

            _bd.InsertarLibro(libro);
            Libros.Add(libro);
            return (true, "");
        }

        public (bool ok, string error) EditarLibro(Libro libro)
        {
            if (!ValidarIsbn(libro.Isbn))
                return (false, "El ISBN-10 no es válido.");

            if (!AnioValido(libro.Anio))
                return (false, $"El año debe estar entre 1500 y {DateTime.Now.Year}.");

            libro.Titulo = FormatearTitulo(libro.Titulo);

            // Comprobar ISBN duplicado (excluyendo el propio libro)
            if (Libros.Any(l => l.Isbn == libro.Isbn && l.Id != libro.Id))
                return (false, "Ya existe otro libro con ese ISBN.");

            _bd.ActualizarLibro(libro);

            // Actualizar en memoria también
            int idx = Libros.FindIndex(l => l.Id == libro.Id);
            if (idx >= 0) Libros[idx] = libro;

            return (true, "");
        }

        public void EliminarLibro(int id)
        {
            _bd.EliminarLibro(id);
            Libros.RemoveAll(l => l.Id == id);
        }

        // ---- CRUD AUDIOLIBROS ----

        public (bool ok, string error) AgregarAudiolibro(Audiolibro audio)
        {
            if (!AnioValido(audio.Anio))
                return (false, $"El año debe estar entre 1500 y {DateTime.Now.Year}.");

            if (audio.FechaFinDisponibilidad < audio.FechaInicioDisponibilidad)
                return (false, "La fecha de fin no puede ser anterior a la de inicio.");

            audio.Titulo = FormatearTitulo(audio.Titulo);

            _bd.InsertarAudiolibro(audio);
            Audiolibros.Add(audio);
            return (true, "");
        }

        public (bool ok, string error) EditarAudiolibro(Audiolibro audio)
        {
            if (!AnioValido(audio.Anio))
                return (false, $"El año debe estar entre 1500 y {DateTime.Now.Year}.");

            if (audio.FechaFinDisponibilidad < audio.FechaInicioDisponibilidad)
                return (false, "La fecha de fin no puede ser anterior a la de inicio.");

            audio.Titulo = FormatearTitulo(audio.Titulo);

            _bd.ActualizarAudiolibro(audio);

            int idx = Audiolibros.FindIndex(a => a.Id == audio.Id);
            if (idx >= 0) Audiolibros[idx] = audio;

            return (true, "");
        }

        public void EliminarAudiolibro(int id)
        {
            _bd.EliminarAudiolibro(id);
            Audiolibros.RemoveAll(a => a.Id == id);
        }

        // ---- PRESTAMOS ----

        public (bool ok, string error) PrestarLibro(int libroId)
        {
            var libro = Libros.Find(l => l.Id == libroId);
            if (libro == null) return (false, "Libro no encontrado.");
            if (!libro.Disponible()) return (false, "El libro ya está prestado.");

            libro.Prestado = true;
            libro.FechaPrestamo = DateTime.Today;
            _bd.ActualizarLibro(libro);
            return (true, "");
        }

        public (bool ok, string error) DevolverLibro(int libroId)
        {
            var libro = Libros.Find(l => l.Id == libroId);
            if (libro == null) return (false, "Libro no encontrado.");
            if (libro.Disponible()) return (false, "El libro no estaba prestado.");

            libro.Prestado = false;
            libro.FechaPrestamo = null;
            _bd.ActualizarLibro(libro);
            return (true, "");
        }

        // ---- VALORACIONES ----

        // Añade una valoracion a un artículo (libro o audiolibro)
        // Devuelve error si la puntuación está fuera de rango
        public (bool ok, string error) AnadirValoracion(int articuloId, string tipo,
            double puntuacion, string idUsuario, string comentario = "", string palabrasClave = "")
        {
            // Validar que la puntuación esté entre 0 y 10
            if (puntuacion < 0 || puntuacion > 10)
                return (false, "La puntuación debe estar entre 0 y 10.");

            if (string.IsNullOrWhiteSpace(idUsuario))
                return (false, "El identificador de usuario es obligatorio.");

            var valoracion = new Valoracion(articuloId, puntuacion, idUsuario, comentario, palabrasClave);
            _bd.InsertarValoracion(valoracion, tipo);

            // Actualizar la lista en memoria
            if (tipo == "libro")
            {
                var libro = Libros.Find(l => l.Id == articuloId);
                libro?.Valoraciones.Add(valoracion);
            }
            else
            {
                var audio = Audiolibros.Find(a => a.Id == articuloId);
                audio?.Valoraciones.Add(valoracion);
            }

            return (true, "");
        }

        // ---- CSV ----

        public void ExportarCSV(string ruta)
        {
            ManejadorCSV.ExportarCatalogo(ruta, Libros, Audiolibros);
        }

        public (int libros, int audiolibros) ImportarCSV(string ruta)
        {
            var (libros, audiolibros) = ManejadorCSV.ImportarCatalogo(ruta);
            int contLibros = 0, contAudios = 0;

            foreach (var libro in libros)
            {
                var (ok, _) = AgregarLibro(libro);
                if (ok) contLibros++;
            }
            foreach (var audio in audiolibros)
            {
                var (ok, _) = AgregarAudiolibro(audio);
                if (ok) contAudios++;
            }

            return (contLibros, contAudios);
        }
    }
}
