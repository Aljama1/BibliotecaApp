-- Script para crear las tablas de la base de datos
-- Se ejecuta automáticamente al arrancar la app, pero lo pongo aquí para documentar

-- Tabla de libros
CREATE TABLE IF NOT EXISTS Libros (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Titulo TEXT NOT NULL,
    Anio INTEGER NOT NULL,
    FechaAdquisicion TEXT NOT NULL,      -- formato YYYY-MM-DD
    Isbn TEXT NOT NULL UNIQUE,           -- ISBN-10, 10 dígitos, único
    Autor TEXT,
    Prestado INTEGER NOT NULL DEFAULT 0, -- 0 = disponible, 1 = prestado
    FechaPrestamo TEXT                   -- null si no está prestado
);

-- Tabla de audiolibros
CREATE TABLE IF NOT EXISTS Audiolibros (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Titulo TEXT NOT NULL,
    Anio INTEGER NOT NULL,
    FechaAdquisicion TEXT NOT NULL,
    Autor TEXT,
    FechaInicioDisponibilidad TEXT NOT NULL,  -- a partir de cuando se puede descargar
    FechaFinDisponibilidad TEXT NOT NULL      -- hasta cuando está disponible
);

-- Tabla de valoraciones (sirve tanto para libros como audiolibros)
-- TipoArticulo indica si la valoracion es de un 'libro' o 'audiolibro'
CREATE TABLE IF NOT EXISTS Valoraciones (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ArticuloId INTEGER NOT NULL,
    TipoArticulo TEXT NOT NULL,   -- 'libro' o 'audiolibro'
    Puntuacion REAL NOT NULL,     -- entre 0.0 y 10.0
    Comentario TEXT,              -- puede ser null
    PalabrasClave TEXT,           -- puede ser null, separadas por comas
    IdUsuario TEXT NOT NULL       -- quien hace la valoracion
);
