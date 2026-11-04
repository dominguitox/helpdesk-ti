IF DB_ID(N'HelpDeskTI') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [HelpDeskTI]');
END;
GO

USE [HelpDeskTI];
GO

DECLARE @DropForeignKeys NVARCHAR(MAX) = N'';

SELECT @DropForeignKeys = @DropForeignKeys
    + N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name)
    + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
    + CHAR(13) + CHAR(10)
FROM sys.foreign_keys AS fk
INNER JOIN sys.tables AS t
    ON t.object_id = fk.parent_object_id
INNER JOIN sys.schemas AS s
    ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0;

IF @DropForeignKeys <> N''
BEGIN
    EXEC sys.sp_executesql @DropForeignKeys;
END;
GO

DECLARE @DropTables NVARCHAR(MAX) = N'';

SELECT @DropTables = @DropTables
    + N'DROP TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N';'
    + CHAR(13) + CHAR(10)
FROM sys.tables AS t
INNER JOIN sys.schemas AS s
    ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0;

IF @DropTables <> N''
BEGIN
    EXEC sys.sp_executesql @DropTables;
END;
GO

CREATE TABLE dbo.Usuarios
(
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) NOT NULL,
    ContrasenaHash CHAR(64) NOT NULL,
    Rol NVARCHAR(20) NOT NULL,
    Activo BIT NOT NULL
        CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    FechaCreacion DATETIME2(0) NOT NULL
        CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
    CONSTRAINT CK_Usuarios_Rol
        CHECK (Rol IN (N'Administrador', N'Usuario', N'Tecnico'))
);
GO

CREATE TABLE dbo.Estados
(
    IdEstado INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(40) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Orden TINYINT NOT NULL,

    CONSTRAINT PK_Estados PRIMARY KEY (IdEstado),
    CONSTRAINT UQ_Estados_Nombre UNIQUE (Nombre),
    CONSTRAINT UQ_Estados_Orden UNIQUE (Orden)
);
GO

CREATE TABLE dbo.Prioridades
(
    IdPrioridad INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(30) NOT NULL,
    Nivel TINYINT NOT NULL,
    Descripcion NVARCHAR(250) NULL,

    CONSTRAINT PK_Prioridades PRIMARY KEY (IdPrioridad),
    CONSTRAINT UQ_Prioridades_Nombre UNIQUE (Nombre),
    CONSTRAINT UQ_Prioridades_Nivel UNIQUE (Nivel)
);
GO

CREATE TABLE dbo.Tecnicos
(
    IdTecnico INT IDENTITY(1,1) NOT NULL,
    IdUsuario INT NOT NULL,
    Activo BIT NOT NULL
        CONSTRAINT DF_Tecnicos_Activo DEFAULT (1),
    FechaIngreso DATETIME2(0) NOT NULL
        CONSTRAINT DF_Tecnicos_FechaIngreso DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Tecnicos PRIMARY KEY (IdTecnico),
    CONSTRAINT UQ_Tecnicos_IdUsuario UNIQUE (IdUsuario),
    CONSTRAINT FK_Tecnicos_Usuarios
        FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario)
);
GO

CREATE TABLE dbo.Tickets
(
    IdTicket INT IDENTITY(1,1) NOT NULL,
    Titulo NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    IdUsuario INT NOT NULL,
    IdTecnico INT NULL,
    IdEstado INT NOT NULL,
    IdPrioridad INT NOT NULL,
    FechaCreacion DATETIME2(0) NOT NULL
        CONSTRAINT DF_Tickets_FechaCreacion DEFAULT (SYSDATETIME()),
    FechaActualizacion DATETIME2(0) NULL,
    FechaCierre DATETIME2(0) NULL,
    Solucion NVARCHAR(MAX) NULL,

    CONSTRAINT PK_Tickets PRIMARY KEY (IdTicket),
    CONSTRAINT FK_Tickets_Usuarios
        FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario),
    CONSTRAINT FK_Tickets_Tecnicos
        FOREIGN KEY (IdTecnico) REFERENCES dbo.Tecnicos (IdTecnico),
    CONSTRAINT FK_Tickets_Estados
        FOREIGN KEY (IdEstado) REFERENCES dbo.Estados (IdEstado),
    CONSTRAINT FK_Tickets_Prioridades
        FOREIGN KEY (IdPrioridad) REFERENCES dbo.Prioridades (IdPrioridad)
);
GO

CREATE INDEX IX_Tickets_IdUsuario
    ON dbo.Tickets (IdUsuario);
GO

CREATE INDEX IX_Tickets_IdTecnico
    ON dbo.Tickets (IdTecnico);
GO

CREATE INDEX IX_Tickets_IdEstado
    ON dbo.Tickets (IdEstado);
GO

CREATE INDEX IX_Tickets_IdPrioridad
    ON dbo.Tickets (IdPrioridad);
GO
