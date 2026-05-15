# Diseño de la Base de Datos - Biblioteca

## Introducción

La base de datos es SQLite, un solo archivo `biblioteca.db` que se genera automáticamente al arrancar la aplicación.
No necesita servidor, lo cual es ideal para una app de escritorio.

## Tablas

### Libros

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INTEGER PK AUTOINCREMENT | Identificador único |
| Titulo | TEXT NOT NULL | Título del libro (primera letra mayúscula) |
| Anio | INTEGER NOT NULL | Año de publicación (1500 - año actual) |
| FechaAdquisicion | TEXT NOT NULL | Fecha de adquisición en formato YYYY-MM-DD |
| Isbn | TEXT NOT NULL UNIQUE | ISBN-10 validado matemáticamente |
| Autor | TEXT | Nombre del autor |
| Prestado | INTEGER NOT NULL DEFAULT 0 | 0=disponible, 1=prestado |
| FechaPrestamo | TEXT | Fecha en que se prestó (NULL si disponible) |

### Audiolibros

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INTEGER PK AUTOINCREMENT | Identificador único |
| Titulo | TEXT NOT NULL | Título del audiolibro |
| Anio | INTEGER NOT NULL | Año de publicación |
| FechaAdquisicion | TEXT NOT NULL | Fecha de adquisición |
| Autor | TEXT | Nombre del autor |
| FechaInicioDisponibilidad | TEXT NOT NULL | Desde cuándo se puede descargar |
| FechaFinDisponibilidad | TEXT NOT NULL | Hasta cuándo está disponible |

### Valoraciones

Una sola tabla para las valoraciones de libros y audiolibros.
El campo `TipoArticulo` distingue a qué tipo pertenece cada valoración.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INTEGER PK AUTOINCREMENT | Identificador único |
| ArticuloId | INTEGER NOT NULL | ID del libro o audiolibro valorado |
| TipoArticulo | TEXT NOT NULL | 'libro' o 'audiolibro' |
| Puntuacion | REAL NOT NULL | Nota entre 0.0 y 10.0 |
| Comentario | TEXT | Comentario opcional |
| PalabrasClave | TEXT | Palabras clave opcionales, separadas por comas |
| IdUsuario | TEXT NOT NULL | Identificador de quien valoró |

## Relaciones

```
Libros (1) ----< Valoraciones (N)   [cuando TipoArticulo = 'libro']
Audiolibros (1) ----< Valoraciones (N)  [cuando TipoArticulo = 'audiolibro']
```

No hay claves foráneas explícitas para simplificar. La relación se gestiona
en la aplicación mediante el par (ArticuloId, TipoArticulo).

## Notas de diseño

- Las fechas se guardan como TEXT en formato ISO 8601 (YYYY-MM-DD) porque
  SQLite no tiene tipo DATE nativo. Al leer, se parsean a DateTime en C#.
- El campo Prestado usa 0/1 en lugar de TRUE/FALSE porque SQLite no tiene
  tipo booleano nativo.
- Se decidió una tabla única para valoraciones en vez de dos tablas separadas
  para simplificar el código (una sola consulta para obtener todas las valoraciones).
