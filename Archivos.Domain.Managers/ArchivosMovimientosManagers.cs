using Archivos.Application.Dto;
using Archivos.Domain.Db;
using Archivos.Domain.Managers.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Archivos.Domain.Managers {
    public class ArchivosMovimientosManagers {
        public List<ArchivosMovimientoDto> ListadoDocumentos(string rutaDestino) {
            using (var context = new ArchivosEntities()) {
                var listadoDb = context.ArchivosMovimientos.AsQueryable();

                listadoDb = listadoDb.Take(20).AsQueryable();

                var listado = listadoDb.Select(s => new ArchivosMovimientoDto() {
                    ArchivosMovimientoCabID = s.ArchivosMovimientoCabID,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    NombreArchivo = s.NombreArchivo,
                    Extension = s.Extension
                }).ToList();

                listado.ForEach(delegate(ArchivosMovimientoDto amDto) {
                    this.RecuperarDocumento(amDto.NombreArchivo, rutaDestino);
                });
                return listado;
            }
        }
        public List<ArchivosMovimientoDto> ListadoDocumentos(string rutaDestino, string titulo, string descripcion) {
            using (var context = new ArchivosEntities()) {
                var listadoDb = context.ArchivosMovimientos.AsQueryable();

                if (titulo != null) {
                    var tituloArray = titulo.Split(',');
                    foreach (var palabra in tituloArray) {
                        var palabraTrim = palabra.Trim();
                        listadoDb = listadoDb.Where(l => l.Titulo.Contains(palabraTrim)).AsQueryable();
                    }
                }
                if (descripcion != null) {
                    var descripcionArray = descripcion.Split(',');
                    foreach (var palabra in descripcionArray) {
                        var palabraTrim = palabra.Trim();
                        listadoDb = listadoDb.Where(l => l.Descripcion.Contains(palabraTrim)).AsQueryable();
                    }
                }

                var listado = listadoDb.Select(s => new ArchivosMovimientoDto() {
                    ArchivosMovimientoCabID = s.ArchivosMovimientoCabID,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    NombreArchivo = s.NombreArchivo,
                    Extension = s.Extension
                }).ToList();

                listado.ForEach(delegate(ArchivosMovimientoDto amDto) {
                    this.RecuperarDocumento(amDto.NombreArchivo, rutaDestino);
                });
                return listado;
            }
        }
        public MensajeDto guardarDocumento(string nombre,
            byte[] contenido,
            string titulo,
            string descripcion,
            string lugarID,
            Guid userID) {
            //Se busca el ID del usuario
            int usuarioIDCarga = 0;
            ArchivosMovimientosCab archivosMovimientosCabsDb;
            using (var context = new ArchivosEntities()) {
                //Se carga el usuario
                var usuarioDb = context.Usuarios
                    .Where(u => u.UserID == userID).FirstOrDefault();
                if (usuarioDb == null) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "Error: Este usuario esta registrado pero no fue aceptado por la administracion aun"
                    };
                }
                usuarioIDCarga = usuarioDb.UsuarioID;
                //Se carga la cabecera
                MensajeDto mensajeDto = null;
                archivosMovimientosCabsDb = new ArchivosMovimientosCab();
                archivosMovimientosCabsDb.Titulo = titulo;

                context.ArchivosMovimientosCabs.Add(archivosMovimientosCabsDb);
                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }
            }

            var mensajeConnString = RecuperarElconnectionStrings("ArchivosDb");
            if (mensajeConnString.Error) {
                return mensajeConnString;
            }
            string CadConexion = mensajeConnString.Valor;

            using (SqlConnection conexionBD = new SqlConnection(CadConexion)) {
                try {
                    conexionBD.Open();
                    if (conexionBD.State == ConnectionState.Open) {
                        using (SqlTransaction transaccion = conexionBD.BeginTransaction()) {
                            //byte[] contenido = File.ReadAllBytes(ubicacion);
                            //Se ve su extencion
                            string[] nombreArchivo = nombre.Split('.');
                            var extension = nombreArchivo[nombreArchivo.Length - 1];

                            string cadSql = @"INSERT INTO [ArchivosMovimientos]
                            (ArchivosMovimientoID,NombreArchivo,
                             DocumentoFile,Titulo, 
                             Descripcion, MomentoCarga,
                             UsuarioIDCarga, Extension,
                             ArchivosMovimientoCabID, LugarID) 
                               VALUES
                            (@ArchivosMovimientoID,@Nombre,
                             @Fichero,@Titulo,
                             @Descripcion, GETDATE(),
                             @UsuarioIDCarga, @Extension,
                             @ArchivosMovimientoCabID, @LugarID)";
                            using (SqlCommand cmd = new SqlCommand(cadSql, conexionBD, transaccion)) {
                                cmd.Parameters.AddWithValue("@ArchivosMovimientoID", Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@Nombre", nombre);
                                cmd.Parameters.AddWithValue("@Fichero", contenido);
                                cmd.Parameters.AddWithValue("@Titulo", titulo);
                                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                                cmd.Parameters.AddWithValue("@UsuarioIDCarga", usuarioIDCarga);
                                cmd.Parameters.AddWithValue("@Extension", extension);
                                cmd.Parameters.AddWithValue("@ArchivosMovimientoCabID", archivosMovimientosCabsDb.ArchivosMovimientoCabID);
                                cmd.Parameters.AddWithValue("@LugarID", lugarID);
                                cmd.ExecuteNonQuery();
                                transaccion.Commit();
                            }
                        }
                    }
                } catch (Exception ex) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "Error: " + ex.Message
                    };
                } finally {
                    if ((conexionBD != null) && (conexionBD.State == ConnectionState.Open)) {
                        conexionBD.Close();
                    }
                }
            }
            return new MensajeDto() {
                Error = false,
                MensajeDelProceso = "Insercion Exitosa del archivo: " + nombre
            };
        }

        #region Auxiliares
        private MensajeDto RecuperarElconnectionStrings(string nombreConeccion) {
            Configuration rootWebConfig;
            rootWebConfig = WebConfigurationManager.OpenWebConfiguration("~/");
            System.Configuration.ConnectionStringSettings connString;
            if (rootWebConfig.AppSettings.Settings.Count > 0) {
                connString = rootWebConfig.ConnectionStrings.ConnectionStrings[nombreConeccion];
                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "connectionString encontrado: " + nombreConeccion,
                    Valor = connString.ToString()
                };
            }
            return new MensajeDto() {
                Error = true,
                MensajeDelProceso = "No se encuentra un nombre de coneccion igual a " + nombreConeccion
            };
        }
        private MensajeDto RecuperarDocumento(string nombre, string rutaDestino) {
            var mensajeConnString = RecuperarElconnectionStrings("ArchivosDb");
            if (mensajeConnString.Error) {
                return mensajeConnString;
            }
            string CadConexion = mensajeConnString.Valor;

            using (SqlConnection conexionBD = new SqlConnection(CadConexion)) {
                try {
                    conexionBD.Open();
                    if (conexionBD.State == ConnectionState.Open) {
                        using (SqlTransaction transaccion = conexionBD.BeginTransaction()) {
                            byte[] arrayContexto = null;
                            string ubicacionFichero = string.Empty;
                            string cadSql = string.Empty;

                            //obtengo el PathName()
                            cadSql = "SELECT DocumentoFile.PathName() from dbo.[ArchivosMovimientos] where NombreArchivo=@nombre";
                            using (SqlCommand cmdUbicacion = new SqlCommand(cadSql, conexionBD, transaccion)) {
                                cmdUbicacion.Parameters.AddWithValue("@Nombre", nombre);
                                object objUbicacion = cmdUbicacion.ExecuteScalar();
                                if (objUbicacion != null) {
                                    ubicacionFichero = objUbicacion.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(ubicacionFichero)) {
                                //todas las operaciones FILESTREAM BLOB deben ocurrir dentro del contexto de una transaccion
                                cadSql = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";
                                using (SqlCommand cmdContexto = new SqlCommand(cadSql, conexionBD, transaccion)) {
                                    object objContexto = cmdContexto.ExecuteScalar();
                                    if (objContexto != null) {
                                        arrayContexto = (byte[])objContexto;
                                    }
                                }

                                //obtener el handle que sera pasado al WIN32 API FILE
                                using (SqlFileStream sqlFS = new SqlFileStream(ubicacionFichero, arrayContexto, FileAccess.Read)) {
                                    byte[] datosFichero = new byte[sqlFS.Length];

                                    sqlFS.Read(datosFichero, 0, (int)sqlFS.Length);
                                    var rutaCompleta = Path.Combine(rutaDestino, nombre);
                                    File.WriteAllBytes(rutaCompleta, datosFichero);
                                    sqlFS.Close();
                                }
                            }
                            transaccion.Commit();
                        }
                    }

                } catch (UnauthorizedAccessException ex) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "No tiene autorizacion para acceder al recurso: " + ex.Message
                    };
                } catch (Exception ex) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "Error: " + ex.Message
                    };
                } finally {
                    if ((conexionBD != null) && (conexionBD.State == ConnectionState.Open)) {
                        conexionBD.Close();
                    }
                }
            }

            return new MensajeDto() {
                Error = false,
                MensajeDelProceso = "Archivo generado",
                Valor = nombre
            };
        }
        #endregion

    }
}
