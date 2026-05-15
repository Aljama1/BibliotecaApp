# Manual de usuario - Aplicación Biblioteca

## Requisitos

- Windows 10/11 (64 bits)
- .NET 9 Runtime instalado

## Cómo ejecutar

```bash
dotnet run
```

O ejecutar directamente el `.exe` de la carpeta `bin/Release/net9.0/`.

## Estructura de la aplicación (MVC)

```
BibliotecaApp/
├── Modelos/          <- Clases de datos (Libro, Audiolibro, etc.)
├── Datos/            <- Acceso a BD y CSV
├── Controlador/      <- Lógica de negocio
├── Vistas/           <- Interfaz gráfica Avalonia
├── sql/              <- Scripts SQL y datos de ejemplo
└── docs/             <- Esta documentación
```

## Funcionalidades

### Ventana principal

Al abrir la app se muestra una tabla con todos los artículos del catálogo.
En la parte superior hay un buscador por título y un filtro por tipo (Todos / Libros / Audiolibros).

### Añadir artículos

- **+ Libro**: Abre un formulario para crear un libro nuevo. Pide título, autor, año, fecha de adquisición e ISBN-10.
- **+ Audiolibro**: Abre un formulario para crear un audiolibro. Pide título, autor, año, fecha de adquisición y periodo de disponibilidad.

### Editar y eliminar

- Selecciona una fila en la tabla y pulsa **Editar** para modificar sus datos.
- Pulsa **Eliminar** para borrarlo (pide confirmación antes).

### Préstamos (solo libros)

- **Prestar**: Marca el libro seleccionado como prestado con la fecha de hoy.
- **Devolver**: Marca el libro como disponible de nuevo.
- El préstamo máximo es de 31 días.

### Valoraciones

- **Valorar**: Añade una valoración al artículo seleccionado. La puntuación (0-10) y el ID de usuario son obligatorios.
- **Ver valoraciones**: Muestra todas las valoraciones del artículo seleccionado con su puntuación media.

### CSV

- **Exportar CSV**: Guarda todo el catálogo actual en un archivo CSV.
- **Importar CSV**: Carga artículos desde un CSV con el mismo formato.

## Validaciones

| Campo | Regla |
|-------|-------|
| Título | Obligatorio. Primera letra en mayúscula automáticamente. |
| Año | Entre 1500 y el año actual. |
| ISBN-10 | 10 dígitos, validación matemática (suma dígito×peso múltiplo de 11). |
| Puntuación | Entre 0 y 10. |
| ID Usuario | Obligatorio en valoraciones. |
| Disponibilidad audiolibro | Fecha fin no puede ser anterior a fecha inicio. |

## Base de datos

La BD se crea automáticamente como `biblioteca.db` en el mismo directorio que el ejecutable.
Para cargar datos de ejemplo ejecutar los scripts de la carpeta `sql/`:

```bash
sqlite3 biblioteca.db < sql/crear_bd.sql
sqlite3 biblioteca.db < sql/datos_iniciales.sql
```

O importar los CSV desde la propia aplicación.
