// Manejador de archivos CSV
// Exporta e importa el catálogo en formato CSV sencillo
// No usamos librería externa, lo hacemos a mano que es más sencillo de entender

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Datos
{
    public class ManejadorCSV
    {
        // Exporta toda la lista de artículos a un archivo CSV
        // Los libros y audiolibros van en el mismo fichero con un campo "tipo"
        public static void ExportarCatalogo(string ruta, List<Libro> libros, List<Audiolibro> audiolibros)
        {
            using var writer = new StreamWriter(ruta, false, Encoding.UTF8);

            // Cabecera del CSV
            writer.WriteLine("tipo,id,titulo,anio,fecha_adquisicion,autor,isbn,prestado," +
                              "fecha_prestamo,fecha_inicio_disp,fecha_fin_disp,valoracion_media");

            // Escribir libros
            foreach (var libro in libros)
            {
                string media = libro.ValoracionMedia().ToString("F2");
                string fechaPrestamo = libro.FechaPrestamo.HasValue
                    ? libro.FechaPrestamo.Value.ToString("yyyy-MM-dd") : "";
                writer.WriteLine(
                    $"libro,{libro.Id},{EscaparCSV(libro.Titulo)},{libro.Anio}," +
                    $"{libro.FechaAdquisicion:yyyy-MM-dd},{EscaparCSV(libro.Autor)}," +
                    $"{libro.Isbn},{libro.Prestado},{fechaPrestamo},,,{media}");
            }

            // Escribir audiolibros
            foreach (var audio in audiolibros)
            {
                string media = audio.ValoracionMedia().ToString("F2");
                writer.WriteLine(
                    $"audiolibro,{audio.Id},{EscaparCSV(audio.Titulo)},{audio.Anio}," +
                    $"{audio.FechaAdquisicion:yyyy-MM-dd},{EscaparCSV(audio.Autor)}," +
                    $",,," +
                    $"{audio.FechaInicioDisponibilidad:yyyy-MM-dd}," +
                    $"{audio.FechaFinDisponibilidad:yyyy-MM-dd},{media}");
            }
        }

        // Importa libros desde un CSV con el mismo formato que exportamos
        public static (List<Libro> libros, List<Audiolibro> audiolibros) ImportarCatalogo(string ruta)
        {
            var libros = new List<Libro>();
            var audiolibros = new List<Audiolibro>();

            if (!File.Exists(ruta))
                return (libros, audiolibros);

            var lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            // Saltamos la primera línea (cabecera)
            for (int i = 1; i < lineas.Length; i++)
            {
                var campos = lineas[i].Split(',');
                if (campos.Length < 12) continue; // línea incompleta, la saltamos

                try
                {
                    string tipo = campos[0].Trim();

                    if (tipo == "libro")
                    {
                        var libro = new Libro(
                            campos[2].Trim('"'),
                            int.Parse(campos[3]),
                            DateTime.Parse(campos[4]),
                            campos[6],
                            campos[5].Trim('"')
                        );
                        libro.Prestado = campos[7] == "True";
                        if (!string.IsNullOrEmpty(campos[8]))
                            libro.FechaPrestamo = DateTime.Parse(campos[8]);
                        libros.Add(libro);
                    }
                    else if (tipo == "audiolibro")
                    {
                        var audio = new Audiolibro(
                            campos[2].Trim('"'),
                            int.Parse(campos[3]),
                            DateTime.Parse(campos[4]),
                            DateTime.Parse(campos[9]),
                            DateTime.Parse(campos[10]),
                            campos[5].Trim('"')
                        );
                        audiolibros.Add(audio);
                    }
                }
                catch (Exception)
                {
                    // Si una línea está mal, la ignoramos y seguimos con la siguiente
                    continue;
                }
            }

            return (libros, audiolibros);
        }

        // Pone comillas en el texto si tiene comas dentro, para que el CSV no se rompa
        private static string EscaparCSV(string texto)
        {
            if (texto.Contains(',') || texto.Contains('"'))
                return $"\"{texto.Replace("\"", "\"\"")}\"";
            return texto;
        }
    }
}
