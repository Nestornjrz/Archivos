using Archivos.Application.Dto;
using Archivos.Domain.Db;
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
                var listado = context.ArchivosMovimientos
                    .Select(s => new ArchivosMovimientoDto() {
                        Titulo = s.Titulo,
                        Descripcion = s.Descripcion,
                        NombreArchivo = s.NombreArchivo
                    }).ToList();
                listado.ForEach(delegate(ArchivosMovimientoDto amDto) {
                    this.RecuperarDocumento(amDto.NombreArchivo, rutaDestino);
                    string[] archivo = amDto.NombreArchivo.Split('.');
                    amDto.Extension = archivo[archivo.Length - 1];
                });

                return listado;
            }
        }
        public MensajeDto guardarDocumento(string nombre,
            byte[] contenido,
            string titulo,
            string descripcion,
            Guid userID) {
            //Se busca el ID del usuario
            int usuarioIDCarga = 0;
            using (var context = new ArchivosEntities()) {
                var usuarioDb = context.Usuarios
                    .Where(u => u.UserID == userID).FirstOrDefault();
                if (usuarioDb == null) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "Error: Este usuario esta registrado pero no fue aceptado por la administracion aun"
                    };
                }
                usuarioIDCarga = usuarioDb.UsuarioID;
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
                             UsuarioIDCarga, Extension) 
                               VALUES
                            (@ArchivosMovimientoID,@Nombre,
                             @Fichero,@Titulo,
                             @Descripcion, GETDATE(),
                             @UsuarioIDCarga, @Extension)";
                            using (SqlCommand cmd = new SqlCommand(cadSql, conexionBD, transaccion)) {
                                cmd.Parameters.AddWithValue("@ArchivosMovimientoID", Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@Nombre", nombre);
                                cmd.Parameters.AddWithValue("@Fichero", contenido);
                                cmd.Parameters.AddWithValue("@Titulo", titulo);
                                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                                cmd.Parameters.AddWithValue("@UsuarioIDCarga", usuarioIDCarga);
                                cmd.Parameters.AddWithValue("@Extension", extension);
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
                            cadSql = "SELECT DocumentoFile.PathName() from dbo.[ArchivosMovimientos] where Nombre=@nombre";
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
