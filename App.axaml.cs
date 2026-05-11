// Punto de entrada de la aplicación Avalonia
// Aquí configuramos la ventana principal y el controlador

using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BibliotecaApp.Controlador;
using BibliotecaApp.Vistas;

namespace BibliotecaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // La BD se guarda en el mismo directorio que el ejecutable
            string rutaBD = Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "biblioteca.db"
            );

            var controlador = new ControladorBiblioteca(rutaBD);
            desktop.MainWindow = new VentanaPrincipal(controlador);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
