// Diálogo para añadir una valoración a un artículo

using Avalonia.Controls;
using BibliotecaApp.Controlador;

namespace BibliotecaApp.Vistas
{
    public partial class DialogoValoracion : Window
    {
        private readonly ControladorBiblioteca _ctrl;
        private readonly int _articuloId;
        private readonly string _tipo;

        public DialogoValoracion(ControladorBiblioteca ctrl, int articuloId, string tipo, string tituloArticulo)
        {
            InitializeComponent();
            _ctrl = ctrl;
            _articuloId = articuloId;
            _tipo = tipo;
            lblTitulo.Text = $"Valorar: {tituloArticulo}";
        }

        private void BtnGuardar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            lblError.IsVisible = false;

            if (!double.TryParse(txtPuntuacion.Text?.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double puntuacion))
            {
                lblError.Text = "La puntuación debe ser un número.";
                lblError.IsVisible = true;
                return;
            }

            var (ok, error) = _ctrl.AnadirValoracion(
                _articuloId, _tipo, puntuacion,
                txtUsuario.Text ?? "",
                txtComentario.Text ?? "",
                txtPalabras.Text ?? ""
            );

            if (!ok)
            {
                lblError.Text = error;
                lblError.IsVisible = true;
                return;
            }

            Close();
        }

        private void BtnCancelar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
