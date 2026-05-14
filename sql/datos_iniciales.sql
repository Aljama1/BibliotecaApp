-- Datos de ejemplo para cargar en la base de datos
-- Ejecutar después de crear_bd.sql

-- Libros de ejemplo
-- Los ISBN-10 son válidos matemáticamente (suma de digito*peso múltiplo de 11)
INSERT INTO Libros (Titulo, Anio, FechaAdquisicion, Isbn, Autor, Prestado, FechaPrestamo)
VALUES
    ('Cien años de soledad',  1967, '2022-01-15', '8420471836', 'Gabriel García Márquez', 0, NULL),
    ('El quijote',            1605, '2021-06-01', '8420400017', 'Miguel de Cervantes',    0, NULL),
    ('La sombra del viento',  2001, '2023-03-10', '8408043641', 'Carlos Ruiz Zafón',      1, '2026-05-10'),
    ('Harry Potter 1',        1997, '2022-09-20', '0439708184', 'J.K. Rowling',           0, NULL),
    ('El principito',         1943, '2020-12-05', '0156013984', 'Antoine de Saint-Exupéry', 0, NULL);

-- Audiolibros de ejemplo
INSERT INTO Audiolibros (Titulo, Anio, FechaAdquisicion, Autor, FechaInicioDisponibilidad, FechaFinDisponibilidad)
VALUES
    ('Sapiens',                  2011, '2023-01-20', 'Yuval Noah Harari',  '2026-01-01', '2026-12-31'),
    ('El poder del ahora',       1997, '2022-07-15', 'Eckhart Tolle',      '2025-06-01', '2026-06-01'),
    ('Padre rico padre pobre',   1997, '2023-05-10', 'Robert Kiyosaki',    '2026-05-01', '2026-08-31'),
    ('Atomic Habits',            2018, '2024-02-01', 'James Clear',        '2024-01-01', '2025-12-31');

-- Valoraciones de ejemplo
INSERT INTO Valoraciones (ArticuloId, TipoArticulo, Puntuacion, Comentario, PalabrasClave, IdUsuario)
VALUES
    (1, 'libro', 9.5, 'Una obra maestra del realismo mágico', 'clasico,latinoamerica', 'usuario001'),
    (1, 'libro', 8.0, 'Muy buena pero algo densa al principio', 'literatura', 'usuario002'),
    (2, 'libro', 9.0, 'El clásico de los clásicos', 'clasico,español', 'usuario003'),
    (4, 'libro', 10.0, 'Genial para todas las edades', 'fantasia,juvenil', 'usuario001'),
    (1, 'audiolibro', 9.0, 'Muy recomendable, cambia la perspectiva', 'historia,humanidad', 'usuario004'),
    (3, 'audiolibro', 7.5, 'Interesante pero algo repetitivo', 'autoayuda', 'usuario002');
