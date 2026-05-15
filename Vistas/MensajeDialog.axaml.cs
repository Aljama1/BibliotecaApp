// Ventana simple de mensaje, para mostrar errores e info

using Avalonia.Controls;

namespace BibliotecaApp.Vistas
{
    public partial class MensajeDialog : Window
    {
        public MensajeDialog(string titulo, string mensaje)
        {
            InitializeComponent();
            Title = titulo;
            lblMensaje.Text = mensaje;
        }

        private void BtnAceptar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
