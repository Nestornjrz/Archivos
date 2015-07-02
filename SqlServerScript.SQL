/*
Created: 24/06/2015
Modified: 01/07/2015
Model: Archivos
Database: MS SQL Server 2014
*/


-- Create tables section -------------------------------------------------

-- Table Usuarios

CREATE TABLE [Usuarios]
(
 [UsuarioID] Int IDENTITY(1,1) NOT NULL,
 [NombreUsuario] Varchar(50) NOT NULL,
 [UserID] Uniqueidentifier NOT NULL,
 [CorreoElectronico] Varchar(50) NOT NULL
)
go

-- Add keys for table Usuarios

ALTER TABLE [Usuarios] ADD CONSTRAINT [pk_Usuarios] PRIMARY KEY ([UsuarioID])
go

-- Table ArchivosMovimientos

CREATE TABLE [ArchivosMovimientos]
(
 [ArchivosMovimientoID] Uniqueidentifier ROWGUIDCOL NOT NULL,
 [ArchivosMovimientoCabID] Bigint NOT NULL,
 [Titulo] Varchar(200) NOT NULL,
 [Descripcion] Text NOT NULL,
 [DocumentoFile] Varbinary(max) FILESTREAM NOT NULL,
 [NombreArchivo] Varchar(100) NOT NULL,
 [Extension] Varchar(5) NOT NULL,
 [LugarID] Int NOT NULL,
 [UsuarioIDCarga] Int NOT NULL,
 [MomentoCarga] Datetime2 NOT NULL,
 CONSTRAINT [Pk_ArchivosMovimientos] PRIMARY KEY NONCLUSTERED ([ArchivosMovimientoID])
)
go

-- Create indexes for table ArchivosMovimientos

CREATE INDEX [IX_ArchivosMovimientos_UsuarioIDCarga] ON [ArchivosMovimientos] ([UsuarioIDCarga])
go

CREATE UNIQUE INDEX [IX_Unique_ArchivosMovimientos_NombreArchivo] ON [ArchivosMovimientos] ([NombreArchivo])
go

CREATE INDEX [IX_ArchivosMovimientos_LugarID] ON [ArchivosMovimientos] ([LugarID])
go

CREATE INDEX [IX_Relationship1] ON [ArchivosMovimientos] ([ArchivosMovimientoCabID])
go

-- Add keys for table ArchivosMovimientos

ALTER TABLE [ArchivosMovimientos] ADD CONSTRAINT [ArchivosMovimientoID] UNIQUE CLUSTERED ([ArchivosMovimientoID])
go

-- Table Lugares

CREATE TABLE [Lugares]
(
 [LugarID] Int IDENTITY(1,1) NOT NULL,
 [NombreLugar] Varchar(50) NOT NULL
)
go

-- Add keys for table Lugares

ALTER TABLE [Lugares] ADD CONSTRAINT [pk_Lugares] PRIMARY KEY ([LugarID])
go

-- Table ArchivosMovimientosCabs

CREATE TABLE [ArchivosMovimientosCabs]
(
 [ArchivosMovimientoCabID] Bigint IDENTITY(1,1) NOT NULL,
 [Titulo] Varchar(200) NOT NULL
)
go

-- Add keys for table ArchivosMovimientosCabs

ALTER TABLE [ArchivosMovimientosCabs] ADD CONSTRAINT [pk_ArchivosMovimientosCabs] PRIMARY KEY ([ArchivosMovimientoCabID])
go

-- Create relationships section ------------------------------------------------- 

ALTER TABLE [ArchivosMovimientos] ADD CONSTRAINT [fk_ArchivosMovimientos>Usuarios] FOREIGN KEY ([UsuarioIDCarga]) REFERENCES [Usuarios] ([UsuarioID]) ON UPDATE NO ACTION ON DELETE NO ACTION
go

ALTER TABLE [ArchivosMovimientos] ADD CONSTRAINT [fk_ArchivosMovimientos>Lugares] FOREIGN KEY ([LugarID]) REFERENCES [Lugares] ([LugarID]) ON UPDATE NO ACTION ON DELETE NO ACTION
go

ALTER TABLE [ArchivosMovimientos] ADD CONSTRAINT [fk_ArchivosMovimientos>ArchivosMovimientosCabs] FOREIGN KEY ([ArchivosMovimientoCabID]) REFERENCES [ArchivosMovimientosCabs] ([ArchivosMovimientoCabID]) ON UPDATE NO ACTION ON DELETE NO ACTION
go



