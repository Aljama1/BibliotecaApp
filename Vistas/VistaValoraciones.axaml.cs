// Ventana que muestra todas las valoraciones de un artículo

using System.Collections.Generic;
using Avalonia.Controls;
using BibliotecaApp.Modelos;

namespace BibliotecaApp.Vistas
{
    public partial class VistaValoraciones : Window
    {
        public VistaValoraciones(string tituloArticulo, List<Valoracion> valoraciones)
        {
            InitializeComponent();
            Title = "Valoraciones";
            lblTitulo.Text = $"Valoraciones de: {tituloArticulo}";
            dgValoraciones.ItemsSource = valoraciones;
        }

        private void BtnCerrar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
