// Diálogo de confirmación, devuelve true/false

using Avalonia.Controls;

namespace BibliotecaApp.Vistas
{
    public partial class ConfirmacionDialog : Window
    {
        public ConfirmacionDialog(string pregunta)
        {
            InitializeComponent();
            Title = "Confirmar";
            lblPregunta.Text = pregunta;
        }

        private void BtnSi_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        private void BtnNo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
