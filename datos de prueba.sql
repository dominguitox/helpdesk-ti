INSERT INTO HelpDeskTI.dbo.Usuarios (Nombre, Correo, ContrasenaHash, Activo)
VALUES
    (N'Administrador', N'admin@empresa.cl',
     CONVERT(CHAR(64), HASHBYTES('SHA2_256', N'12345'), 2), 1),
    (N'Pepito', N'pepito@empresa.cl',
     CONVERT(CHAR(64), HASHBYTES('SHA2_256', N'12345'), 2), 1),
    (N'Usuario', N'eldiosdelsixseven67@gmail.com',
     CONVERT(CHAR(64), HASHBYTES('SHA2_256', N'12345'), 2), 1);

INSERT INTO HelpDeskTI.dbo.Administradores (IdUsuario, Activo)
SELECT IdUsuario, 1
FROM HelpDeskTI.dbo.Usuarios
WHERE Correo = N'admin@empresa.cl';

INSERT INTO HelpDeskTI.dbo.Tecnicos (IdUsuario, Activo)
SELECT IdUsuario, 1
FROM HelpDeskTI.dbo.Usuarios
WHERE Correo = N'pepito@empresa.cl';

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

INSERT INTO HelpDeskTI.dbo.Tickets
    (Titulo, Descripcion, IdUsuario, IdTecnico, IdEstado, IdPrioridad,
     FechaCreacion, FechaActualizacion, FechaCierre, Solucion)
SELECT N'No puedo conectarme al Wi-Fi',
       N'El equipo no logra conectarse a la red de la oficina.',
       usuario.IdUsuario, tecnico.IdTecnico, estado.IdEstado, prioridad.IdPrioridad,
       DATEADD(DAY, -2, SYSDATETIME()), DATEADD(HOUR, -1, SYSDATETIME()), NULL, NULL
FROM HelpDeskTI.dbo.Usuarios AS usuario
CROSS JOIN HelpDeskTI.dbo.Tecnicos AS tecnico
CROSS JOIN HelpDeskTI.dbo.Estados AS estado
CROSS JOIN HelpDeskTI.dbo.Prioridades AS prioridad
WHERE usuario.Correo = N'eldiosdelsixseven67@gmail.com'
  AND tecnico.IdUsuario = (SELECT IdUsuario FROM HelpDeskTI.dbo.Usuarios WHERE Correo = N'pepito@empresa.cl')
  AND estado.Nombre = N'En proceso'
  AND prioridad.Nombre = N'Alta'
UNION ALL
SELECT N'Solicitud de instalacion de Office',
       N'Se requiere instalar el paquete de ofimatica.',
       usuario.IdUsuario, NULL, estado.IdEstado, prioridad.IdPrioridad,
       SYSDATETIME(), NULL, NULL, NULL
FROM HelpDeskTI.dbo.Usuarios AS usuario
CROSS JOIN HelpDeskTI.dbo.Estados AS estado
CROSS JOIN HelpDeskTI.dbo.Prioridades AS prioridad
WHERE usuario.Correo = N'eldiosdelsixseven67@gmail.com'
  AND estado.Nombre = N'Nuevo'
  AND prioridad.Nombre = N'Media'
UNION ALL
SELECT N'Equipo no enciende',
       N'El computador no inicia al presionar el boton de encendido.',
       usuario.IdUsuario, tecnico.IdTecnico, estado.IdEstado, prioridad.IdPrioridad,
       DATEADD(DAY, -1, SYSDATETIME()), DATEADD(MINUTE, -30, SYSDATETIME()), NULL, NULL
FROM HelpDeskTI.dbo.Usuarios AS usuario
CROSS JOIN HelpDeskTI.dbo.Tecnicos AS tecnico
CROSS JOIN HelpDeskTI.dbo.Estados AS estado
CROSS JOIN HelpDeskTI.dbo.Prioridades AS prioridad
WHERE usuario.Correo = N'eldiosdelsixseven67@gmail.com'
  AND tecnico.IdUsuario = (SELECT IdUsuario FROM HelpDeskTI.dbo.Usuarios WHERE Correo = N'pepito@empresa.cl')
  AND estado.Nombre = N'Asignado'
  AND prioridad.Nombre = N'Critica'
UNION ALL
SELECT N'Problema de acceso al correo',
       N'El usuario no podia acceder a su correo corporativo.',
       usuario.IdUsuario, tecnico.IdTecnico, estado.IdEstado, prioridad.IdPrioridad,
       DATEADD(DAY, -2, SYSDATETIME()), DATEADD(HOUR, -1, SYSDATETIME()),
       DATEADD(MINUTE, -30, SYSDATETIME()),
       N'Se restablecio el acceso al correo corporativo.'
FROM HelpDeskTI.dbo.Usuarios AS usuario
CROSS JOIN HelpDeskTI.dbo.Tecnicos AS tecnico
CROSS JOIN HelpDeskTI.dbo.Estados AS estado
CROSS JOIN HelpDeskTI.dbo.Prioridades AS prioridad
WHERE usuario.Correo = N'eldiosdelsixseven67@gmail.com'
  AND tecnico.IdUsuario = (SELECT IdUsuario FROM HelpDeskTI.dbo.Usuarios WHERE Correo = N'pepito@empresa.cl')
  AND estado.Nombre = N'Resuelto'
  AND prioridad.Nombre = N'Alta'
UNION ALL
SELECT N'Impresora no responde',
       N'La impresora no responde a los trabajos enviados.',
       usuario.IdUsuario, tecnico.IdTecnico, estado.IdEstado, prioridad.IdPrioridad,
       DATEADD(HOUR, -3, SYSDATETIME()), DATEADD(HOUR, -1, SYSDATETIME()), NULL, NULL
FROM HelpDeskTI.dbo.Usuarios AS usuario
CROSS JOIN HelpDeskTI.dbo.Tecnicos AS tecnico
CROSS JOIN HelpDeskTI.dbo.Estados AS estado
CROSS JOIN HelpDeskTI.dbo.Prioridades AS prioridad
WHERE usuario.Correo = N'eldiosdelsixseven67@gmail.com'
  AND tecnico.IdUsuario = (SELECT IdUsuario FROM HelpDeskTI.dbo.Usuarios WHERE Correo = N'pepito@empresa.cl')
  AND estado.Nombre = N'En espera'
  AND prioridad.Nombre = N'Baja';
