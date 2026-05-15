// Diálogo para crear o editar un artículo (libro o audiolibro)
// Si articuloEditar es null => modo crear, si tiene valor => modo editar

using System;
using Avalonia.Controls;
using BibliotecaApp.Controlador;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Vistas
{
    public partial class DialogoArticulo : Window
    {
        private readonly ControladorBiblioteca _ctrl;
        private readonly Articulo? _articuloEditar; // null si es nuevo
        private readonly string _tipo; // "libro" o "audiolibro"

        public DialogoArticulo(ControladorBiblioteca ctrl, Articulo? articuloEditar, string tipo)
        {
            InitializeComponent();
            _ctrl = ctrl;
            _articuloEditar = articuloEditar;
            _tipo = tipo;

            // Mostrar/ocultar campos según el tipo
            if (tipo == "audiolibro")
            {
                panelLibro.IsVisible = false;
                panelAudio.IsVisible = true;
                Title = articuloEditar == null ? "Nuevo Audiolibro" : "Editar Audiolibro";
            }
            else
            {
                Title = articuloEditar == null ? "Nuevo Libro" : "Editar Libro";
            }

            // Si editamos, rellenar los campos con los datos actuales
            if (articuloEditar != null)
                CargarDatos(articuloEditar);
            else
                dpFechaAdq.SelectedDate = DateTimeOffset.Now; // fecha de hoy por defecto
        }

        // Rellena los campos del formulario con los datos del artículo a editar
        private void CargarDatos(Articulo art)
        {
            txtTitulo.Text = art.Titulo;
            txtAnio.Text = art.Anio.ToString();
            dpFechaAdq.SelectedDate = new DateTimeOffset(art.FechaAdquisicion);

            if (art is Libro libro)
            {
                txtAutor.Text = libro.Autor;
                txtIsbn.Text = libro.Isbn;
            }
            else if (art is Audiolibro audio)
            {
                txtAutor.Text = audio.Autor;
                dpInicio.SelectedDate = new DateTimeOffset(audio.FechaInicioDisponibilidad);
                dpFin.SelectedDate = new DateTimeOffset(audio.FechaFinDisponibilidad);
            }
        }

        private async void BtnGuardar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            lblError.IsVisible = false;

            // Validar campos básicos
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MostrarError("El título es obligatorio.");
                return;
            }

            if (!int.TryParse(txtAnio.Text, out int anio))
            {
                MostrarError("El año debe ser un número.");
                return;
            }

            if (dpFechaAdq.SelectedDate == null)
            {
                MostrarError("La fecha de adquisición es obligatoria.");
                return;
            }

            DateTime fechaAdq = dpFechaAdq.SelectedDate.Value.DateTime;
            string titulo = txtTitulo.Text;
            string autor = txtAutor.Text ?? "";

            (bool ok, string error) resultado;

            if (_tipo == "libro")
            {
                var libro = new Libro(titulo, anio, fechaAdq, txtIsbn.Text ?? "", autor);

                if (_articuloEditar != null)
                {
                    // Modo editar: mantener el ID y el estado de préstamo
                    libro.Id = _articuloEditar.Id;
                    var libroOriginal = (Libro)_articuloEditar;
                    libro.Prestado = libroOriginal.Prestado;
                    libro.FechaPrestamo = libroOriginal.FechaPrestamo;
                    resultado = _ctrl.EditarLibro(libro);
                }
                else
                {
                    resultado = _ctrl.AgregarLibro(libro);
                }
            }
            else // audiolibro
            {
                if (dpInicio.SelectedDate == null || dpFin.SelectedDate == null)
                {
                    MostrarError("Las fechas de disponibilidad son obligatorias.");
                    return;
                }

                var audio = new Audiolibro(titulo, anio, fechaAdq,
                    dpInicio.SelectedDate.Value.DateTime,
                    dpFin.SelectedDate.Value.DateTime, autor);

                if (_articuloEditar != null)
                {
                    audio.Id = _articuloEditar.Id;
                    resultado = _ctrl.EditarAudiolibro(audio);
                }
                else
                {
                    resultado = _ctrl.AgregarAudiolibro(audio);
                }
            }

            if (!resultado.ok)
            {
                MostrarError(resultado.error);
                return;
            }

            Close();
        }

        private void BtnCancelar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.IsVisible = true;
        }
    }
}
