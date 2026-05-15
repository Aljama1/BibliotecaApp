// Code-behind de la ventana principal
// Aquí gestionamos los eventos de los botones y actualizamos la tabla

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BibliotecaApp.Controlador;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Vistas
{
    public partial class VentanaPrincipal : Window
    {
        private readonly ControladorBiblioteca _ctrl;

        // Clase auxiliar solo para mostrar datos en la tabla
        // Mezcla libros y audiolibros en una fila común
        private class FilaCatalogo
        {
            public string Tipo { get; set; } = "";
            public int Id { get; set; }
            public string Titulo { get; set; } = "";
            public string Autor { get; set; } = "";
            public int Anio { get; set; }
            public string Isbn { get; set; } = "";
            public string Estado { get; set; } = "";
            public string ValMedia { get; set; } = "";
            public string FechaAdq { get; set; } = "";
            // Guardamos referencia al artículo original para poder editarlo/borrarlo
            public Articulo ArticuloOriginal { get; set; } = null!;
        }

        public VentanaPrincipal(ControladorBiblioteca ctrl)
        {
            InitializeComponent();
            _ctrl = ctrl;
            RefrescarTabla();
        }

        // Rellena la tabla con los artículos actuales (respetando filtros si los hay)
        private void RefrescarTabla(string busqueda = "", string tipo = "Todos")
        {
            var filas = new List<FilaCatalogo>();

            // Obtener artículos según filtros
            var filtros = new Dictionary<string, object>();
            if (tipo == "Libros") filtros["tipo"] = "libro";
            else if (tipo == "Audiolibros") filtros["tipo"] = "audiolibro";

            string[] titulos = string.IsNullOrWhiteSpace(busqueda)
                ? Array.Empty<string>()
                : new[] { busqueda };

            var articulos = _ctrl.BuscarItems(titulos, filtros);

            foreach (var art in articulos)
            {
                var fila = new FilaCatalogo
                {
                    Id = art.Id,
                    Titulo = art.Titulo,
                    Anio = art.Anio,
                    FechaAdq = art.FechaAdquisicion.ToString("dd/MM/yyyy"),
                    ArticuloOriginal = art
                };

                if (art is Libro libro)
                {
                    fila.Tipo = "Libro";
                    fila.Autor = libro.Autor;
                    fila.Isbn = libro.Isbn;
                    fila.Estado = libro.Prestado ? "Prestado" : "Disponible";
                    fila.ValMedia = libro.ValoracionMedia() > 0
                        ? libro.ValoracionMedia().ToString("F1")
                        : "Sin valorar";
                }
                else if (art is Audiolibro audio)
                {
                    fila.Tipo = "Audiolibro";
                    fila.Autor = audio.Autor;
                    fila.Estado = audio.EstaDisponible() ? "Disponible" : "No disponible";
                    fila.ValMedia = audio.ValoracionMedia() > 0
                        ? audio.ValoracionMedia().ToString("F1")
                        : "Sin valorar";
                }

                filas.Add(fila);
            }

            dgCatalogo.ItemsSource = filas;
        }

        // Devuelve la fila seleccionada en la tabla, o null si no hay ninguna
        private FilaCatalogo? FilaSeleccionada()
        {
            return dgCatalogo.SelectedItem as FilaCatalogo;
        }

        // ---- EVENTOS DE BÚSQUEDA ----

        private void TxtBuscar_TextChanged(object? sender, TextChangedEventArgs e)
        {
            RefrescarTabla(txtBuscar.Text ?? "", (cmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos");
        }

        private void CmbTipo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            RefrescarTabla(txtBuscar.Text ?? "", (cmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos");
        }

        private void BtnLimpiar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            txtBuscar.Text = "";
            cmbTipo.SelectedIndex = 0;
            RefrescarTabla();
        }

        // ---- CRUD ----

        private async void BtnNuevoLibro_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialogo = new DialogoArticulo(_ctrl, null, "libro");
            await dialogo.ShowDialog(this);
            RefrescarTabla();
        }

        private async void BtnNuevoAudio_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialogo = new DialogoArticulo(_ctrl, null, "audiolibro");
            await dialogo.ShowDialog(this);
            RefrescarTabla();
        }

        private async void BtnEditar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null)
            {
                await MostrarError("Selecciona un artículo para editar.");
                return;
            }

            string tipo = fila.ArticuloOriginal is Libro ? "libro" : "audiolibro";
            var dialogo = new DialogoArticulo(_ctrl, fila.ArticuloOriginal, tipo);
            await dialogo.ShowDialog(this);
            RefrescarTabla();
        }

        private async void BtnEliminar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null)
            {
                await MostrarError("Selecciona un artículo para eliminar.");
                return;
            }

            // Confirmación antes de borrar
            var confirmacion = new ConfirmacionDialog($"¿Eliminar '{fila.Titulo}'?");
            bool confirmar = await confirmacion.ShowDialog<bool>(this);
            if (!confirmar) return;

            if (fila.ArticuloOriginal is Libro)
                _ctrl.EliminarLibro(fila.Id);
            else
                _ctrl.EliminarAudiolibro(fila.Id);

            RefrescarTabla();
        }

        // ---- PRÉSTAMOS ----

        private async void BtnPrestar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null || fila.ArticuloOriginal is not Libro)
            {
                await MostrarError("Selecciona un libro para prestar.");
                return;
            }

            var (ok, error) = _ctrl.PrestarLibro(fila.Id);
            if (!ok) await MostrarError(error);
            RefrescarTabla();
        }

        private async void BtnDevolver_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null || fila.ArticuloOriginal is not Libro)
            {
                await MostrarError("Selecciona un libro para devolver.");
                return;
            }

            var (ok, error) = _ctrl.DevolverLibro(fila.Id);
            if (!ok) await MostrarError(error);
            RefrescarTabla();
        }

        // ---- VALORACIONES ----

        private async void BtnValoracion_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null)
            {
                await MostrarError("Selecciona un artículo para valorar.");
                return;
            }

            string tipo = fila.ArticuloOriginal is Libro ? "libro" : "audiolibro";
            var dialogo = new DialogoValoracion(_ctrl, fila.Id, tipo, fila.Titulo);
            await dialogo.ShowDialog(this);
            RefrescarTabla();
        }

        private async void BtnVerValoraciones_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var fila = FilaSeleccionada();
            if (fila == null)
            {
                await MostrarError("Selecciona un artículo para ver sus valoraciones.");
                return;
            }

            // Recargar valoraciones desde la BD para tenerlas actualizadas
            List<Valoracion> vals;
            if (fila.ArticuloOriginal is Libro lb)
                vals = lb.Valoraciones;
            else if (fila.ArticuloOriginal is Audiolibro au)
                vals = au.Valoraciones;
            else
                vals = new();

            var dialogo = new VistaValoraciones(fila.Titulo, vals);
            await dialogo.ShowDialog(this);
        }

        // ---- CSV ----

        private async void BtnExportarCSV_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var opciones = new FilePickerSaveOptions
            {
                Title = "Exportar catálogo CSV",
                SuggestedFileName = "catalogo.csv",
                FileTypeChoices = new[] { new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } } }
            };

            var archivo = await StorageProvider.SaveFilePickerAsync(opciones);
            if (archivo == null) return;

            try
            {
                _ctrl.ExportarCSV(archivo.Path.LocalPath);
                await MostrarInfo("Catálogo exportado correctamente.");
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al exportar: {ex.Message}");
            }
        }

        private async void BtnImportarCSV_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var opciones = new FilePickerOpenOptions
            {
                Title = "Importar catálogo CSV",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } } }
            };

            var archivos = await StorageProvider.OpenFilePickerAsync(opciones);
            if (archivos.Count == 0) return;

            try
            {
                var (libros, audios) = _ctrl.ImportarCSV(archivos[0].Path.LocalPath);
                await MostrarInfo($"Importados: {libros} libros, {audios} audiolibros.");
                RefrescarTabla();
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al importar: {ex.Message}");
            }
        }

        // ---- AYUDAS ----

        private async System.Threading.Tasks.Task MostrarError(string mensaje)
        {
            var dlg = new MensajeDialog("Error", mensaje);
            await dlg.ShowDialog(this);
        }

        private async System.Threading.Tasks.Task MostrarInfo(string mensaje)
        {
            var dlg = new MensajeDialog("Info", mensaje);
            await dlg.ShowDialog(this);
        }
    }
}
