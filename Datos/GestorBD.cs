// Gestor de la base de datos SQLite
// Aquí van todas las operaciones con la BD: crear tablas, insertar, buscar, etc.
// Usamos Microsoft.Data.Sqlite para conectar con el archivo .db

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Datos
{
    public class GestorBD
    {
        // Ruta al archivo de la base de datos
        private readonly string _rutaBD;

        public GestorBD(string rutaBD)
        {
            _rutaBD = rutaBD;
        }

        // Crea y devuelve la conexión a la base de datos
        // Si el archivo no existe, SQLite lo crea automáticamente
        public SqliteConnection CrearConexionBD()
        {
            var conexion = new SqliteConnection($"Data Source={_rutaBD}");
            conexion.Open();
            return conexion;
        }

        // Crea las tablas si no existen
        // Se llama al arrancar la app para asegurarnos de que la BD está lista
        public void InicializarBD(SqliteConnection conexion)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Libros (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titulo TEXT NOT NULL,
                    Anio INTEGER NOT NULL,
                    FechaAdquisicion TEXT NOT NULL,
                    Isbn TEXT NOT NULL UNIQUE,
                    Autor TEXT,
                    Prestado INTEGER NOT NULL DEFAULT 0,
                    FechaPrestamo TEXT
                );

                CREATE TABLE IF NOT EXISTS Audiolibros (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titulo TEXT NOT NULL,
                    Anio INTEGER NOT NULL,
                    FechaAdquisicion TEXT NOT NULL,
                    Autor TEXT,
                    FechaInicioDisponibilidad TEXT NOT NULL,
                    FechaFinDisponibilidad TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Valoraciones (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ArticuloId INTEGER NOT NULL,
                    TipoArticulo TEXT NOT NULL,
                    Puntuacion REAL NOT NULL,
                    Comentario TEXT,
                    PalabrasClave TEXT,
                    IdUsuario TEXT NOT NULL
                );
            ";

            using var cmd = new SqliteCommand(sql, conexion);
            cmd.ExecuteNonQuery();
        }

        // ---- LIBROS ----

        public void InsertarLibro(Libro libro)
        {
            using var con = CrearConexionBD();
            string sql = @"INSERT INTO Libros (Titulo, Anio, FechaAdquisicion, Isbn, Autor, Prestado, FechaPrestamo)
                           VALUES (@titulo, @anio, @fecha, @isbn, @autor, @prestado, @fechaPrestamo)";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@titulo", libro.Titulo);
            cmd.Parameters.AddWithValue("@anio", libro.Anio);
            cmd.Parameters.AddWithValue("@fecha", libro.FechaAdquisicion.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@isbn", libro.Isbn);
            cmd.Parameters.AddWithValue("@autor", libro.Autor);
            cmd.Parameters.AddWithValue("@prestado", libro.Prestado ? 1 : 0);
            cmd.Parameters.AddWithValue("@fechaPrestamo",
                libro.FechaPrestamo.HasValue ? libro.FechaPrestamo.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.ExecuteNonQuery();

            // Recuperar el ID generado por la BD
            cmd.CommandText = "SELECT last_insert_rowid()";
            libro.Id = (int)(long)cmd.ExecuteScalar()!;
        }

        public void ActualizarLibro(Libro libro)
        {
            using var con = CrearConexionBD();
            string sql = @"UPDATE Libros SET Titulo=@titulo, Anio=@anio, FechaAdquisicion=@fecha,
                           Isbn=@isbn, Autor=@autor, Prestado=@prestado, FechaPrestamo=@fechaPrestamo
                           WHERE Id=@id";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@titulo", libro.Titulo);
            cmd.Parameters.AddWithValue("@anio", libro.Anio);
            cmd.Parameters.AddWithValue("@fecha", libro.FechaAdquisicion.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@isbn", libro.Isbn);
            cmd.Parameters.AddWithValue("@autor", libro.Autor);
            cmd.Parameters.AddWithValue("@prestado", libro.Prestado ? 1 : 0);
            cmd.Parameters.AddWithValue("@fechaPrestamo",
                libro.FechaPrestamo.HasValue ? libro.FechaPrestamo.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.Parameters.AddWithValue("@id", libro.Id);
            cmd.ExecuteNonQuery();
        }

        public void EliminarLibro(int id)
        {
            using var con = CrearConexionBD();
            using var cmd = new SqliteCommand("DELETE FROM Libros WHERE Id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            // También borramos sus valoraciones
            cmd.CommandText = "DELETE FROM Valoraciones WHERE ArticuloId=@id AND TipoArticulo='libro'";
            cmd.ExecuteNonQuery();
        }

        public List<Libro> ObtenerLibros()
        {
            var lista = new List<Libro>();
            using var con = CrearConexionBD();
            using var cmd = new SqliteCommand("SELECT * FROM Libros", con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var libro = new Libro(
                    reader.GetString(1),
                    reader.GetInt32(2),
                    DateTime.Parse(reader.GetString(3)),
                    reader.GetString(4),
                    reader.IsDBNull(5) ? "" : reader.GetString(5)
                );
                libro.Id = reader.GetInt32(0);
                libro.Prestado = reader.GetInt32(6) == 1;
                libro.FechaPrestamo = reader.IsDBNull(7) ? null : DateTime.Parse(reader.GetString(7));
                lista.Add(libro);
            }
            return lista;
        }

        // ---- AUDIOLIBROS ----

        public void InsertarAudiolibro(Audiolibro audio)
        {
            using var con = CrearConexionBD();
            string sql = @"INSERT INTO Audiolibros (Titulo, Anio, FechaAdquisicion, Autor, FechaInicioDisponibilidad, FechaFinDisponibilidad)
                           VALUES (@titulo, @anio, @fecha, @autor, @inicio, @fin)";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@titulo", audio.Titulo);
            cmd.Parameters.AddWithValue("@anio", audio.Anio);
            cmd.Parameters.AddWithValue("@fecha", audio.FechaAdquisicion.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@autor", audio.Autor);
            cmd.Parameters.AddWithValue("@inicio", audio.FechaInicioDisponibilidad.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fin", audio.FechaFinDisponibilidad.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            audio.Id = (int)(long)cmd.ExecuteScalar()!;
        }

        public void ActualizarAudiolibro(Audiolibro audio)
        {
            using var con = CrearConexionBD();
            string sql = @"UPDATE Audiolibros SET Titulo=@titulo, Anio=@anio, FechaAdquisicion=@fecha,
                           Autor=@autor, FechaInicioDisponibilidad=@inicio, FechaFinDisponibilidad=@fin
                           WHERE Id=@id";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@titulo", audio.Titulo);
            cmd.Parameters.AddWithValue("@anio", audio.Anio);
            cmd.Parameters.AddWithValue("@fecha", audio.FechaAdquisicion.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@autor", audio.Autor);
            cmd.Parameters.AddWithValue("@inicio", audio.FechaInicioDisponibilidad.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fin", audio.FechaFinDisponibilidad.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@id", audio.Id);
            cmd.ExecuteNonQuery();
        }

        public void EliminarAudiolibro(int id)
        {
            using var con = CrearConexionBD();
            using var cmd = new SqliteCommand("DELETE FROM Audiolibros WHERE Id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DELETE FROM Valoraciones WHERE ArticuloId=@id AND TipoArticulo='audiolibro'";
            cmd.ExecuteNonQuery();
        }

        public List<Audiolibro> ObtenerAudiolibros()
        {
            var lista = new List<Audiolibro>();
            using var con = CrearConexionBD();
            using var cmd = new SqliteCommand("SELECT * FROM Audiolibros", con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var audio = new Audiolibro(
                    reader.GetString(1),
                    reader.GetInt32(2),
                    DateTime.Parse(reader.GetString(3)),
                    DateTime.Parse(reader.GetString(5)),
                    DateTime.Parse(reader.GetString(6)),
                    reader.IsDBNull(4) ? "" : reader.GetString(4)
                );
                audio.Id = reader.GetInt32(0);
                lista.Add(audio);
            }
            return lista;
        }

        // ---- VALORACIONES ----

        public void InsertarValoracion(Valoracion v, string tipo)
        {
            using var con = CrearConexionBD();
            string sql = @"INSERT INTO Valoraciones (ArticuloId, TipoArticulo, Puntuacion, Comentario, PalabrasClave, IdUsuario)
                           VALUES (@artId, @tipo, @punt, @com, @pal, @user)";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@artId", v.ArticuloId);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            cmd.Parameters.AddWithValue("@punt", v.Puntuacion);
            cmd.Parameters.AddWithValue("@com", v.Comentario);
            cmd.Parameters.AddWithValue("@pal", v.PalabrasClave);
            cmd.Parameters.AddWithValue("@user", v.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        public List<Valoracion> ObtenerValoraciones(int articuloId, string tipo)
        {
            var lista = new List<Valoracion>();
            using var con = CrearConexionBD();
            string sql = "SELECT * FROM Valoraciones WHERE ArticuloId=@id AND TipoArticulo=@tipo";
            using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", articuloId);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var v = new Valoracion(
                    reader.GetInt32(1),
                    reader.GetDouble(3),
                    reader.GetString(6),
                    reader.IsDBNull(4) ? "" : reader.GetString(4),
                    reader.IsDBNull(5) ? "" : reader.GetString(5)
                );
                v.Id = reader.GetInt32(0);
                lista.Add(v);
            }
            return lista;
        }
    }
}
