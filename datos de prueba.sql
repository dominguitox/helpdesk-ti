INSERT INTO HelpDeskTI.dbo.Usuarios
    (Nombre, Correo, ContrasenaHash, Rol, Activo, FechaCreacion)
VALUES
    (N'Administrador', N'admin@empresa.cl',
     CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'12345'), 2),
     N'Administrador', 1, SYSDATETIME()),
    (N'Pepito', N'pepito@empresa.cl',
     CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'12345'), 2),
     N'Tecnico', 1, SYSDATETIME()),
    (N'Usuario', N'eldiosdelsixseven67@gmail.com',
     CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', N'12345'), 2),
     N'Usuario', 1, SYSDATETIME());

INSERT INTO HelpDeskTI.dbo.Estados (Nombre, Descripcion, Orden)
VALUES
    (N'Nuevo', N'Ticket recien registrado.', 1),
    (N'Asignado', N'Ticket asignado a un tecnico.', 2),
    (N'En proceso', N'Ticket en proceso de atencion.', 3),
    (N'En espera', N'Ticket pendiente de informacion o recursos.', 4),
    (N'Resuelto', N'Problema solucionado.', 5),
    (N'Cerrado', N'Ticket finalizado.', 6);

INSERT INTO HelpDeskTI.dbo.Prioridades (Nombre, Nivel, Descripcion)
VALUES
    (N'Baja', 1, N'Solicitud de baja urgencia.'),
    (N'Media', 2, N'Solicitud de urgencia normal.'),
    (N'Alta', 3, N'Solicitud que requiere atencion prioritaria.'),
    (N'Critica', 4, N'Incidente urgente o de alto impacto.');

INSERT INTO HelpDeskTI.dbo.Tecnicos (IdUsuario, Activo)
VALUES
    (2, 1);

INSERT INTO HelpDeskTI.dbo.Tickets
    (Titulo, Descripcion, IdUsuario, IdTecnico, IdEstado, IdPrioridad,
     FechaCreacion, FechaActualizacion, FechaCierre, Solucion)
VALUES
    (N'No puedo conectarme al Wi-Fi',
     N'El equipo no logra conectarse a la red de la oficina.',
     3, 1, 3, 3,
     DATEADD(DAY, -2, SYSDATETIME()),
     DATEADD(HOUR, -1, SYSDATETIME()),
     CAST(NULL AS DATETIME2(0)), CAST(NULL AS NVARCHAR(MAX))),
    (N'Solicitud de instalacion de Office',
     N'Se requiere instalar el paquete de ofimatica.',
     3, CAST(NULL AS INT), 1, 2,
     SYSDATETIME(),
     CAST(NULL AS DATETIME2(0)), CAST(NULL AS DATETIME2(0)),
     CAST(NULL AS NVARCHAR(MAX))),
    (N'Equipo no enciende',
     N'El computador no inicia al presionar el boton de encendido.',
     3, 1, 2, 4,
     DATEADD(DAY, -1, SYSDATETIME()),
     DATEADD(MINUTE, -30, SYSDATETIME()),
     CAST(NULL AS DATETIME2(0)), CAST(NULL AS NVARCHAR(MAX))),
    (N'Problema de acceso al correo',
     N'El usuario no podia acceder a su correo corporativo.',
     3, 1, 5, 3,
     DATEADD(DAY, -2, SYSDATETIME()),
     DATEADD(HOUR, -1, SYSDATETIME()),
     DATEADD(MINUTE, -30, SYSDATETIME()),
     N'Se restablecio el acceso al correo corporativo.'),
    (N'Impresora no responde',
     N'La impresora no responde a los trabajos enviados.',
     3, 1, 4, 1,
     DATEADD(HOUR, -3, SYSDATETIME()),
     DATEADD(HOUR, -1, SYSDATETIME()),
     CAST(NULL AS DATETIME2(0)), CAST(NULL AS NVARCHAR(MAX)));
