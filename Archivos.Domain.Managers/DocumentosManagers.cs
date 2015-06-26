using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using Archivos.Application.Dto;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Data.SqlTypes;
using Archivos.Domain.Db;

namespace Archivos.Domain.Managers {
    public class DocumentosManagers {


        public MensajeDto guardarDocumento(string nombre, byte[] contenido) {
            var mensajeConnString = RecuperarElconnectionStrings("ArchivosDb");

            string CadConexion = mensajeConnString.Valor;

            using (SqlConnection conexionBD = new SqlConnection(CadConexion)) {
                try {
                    conexionBD.Open();
                    if (conexionBD.State == ConnectionState.Open) {
                        using (SqlTransaction transaccion = conexionBD.BeginTransaction()) {
                            //byte[] contenido = File.ReadAllBytes(ubicacion);

                            string cadSql = "INSERT INTO [Documentos](Id,Nombre,DocumentoFile) VALUES(@Id,@Nombre,@Fichero)";
                            using (SqlCommand cmd = new SqlCommand(cadSql, conexionBD, transaccion)) {
                                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@Nombre", nombre);
                                cmd.Parameters.AddWithValue("@Fichero", contenido);
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
                            cadSql = "SELECT DocumentoFile.PathName() from dbo.[Documentos] where Nombre=@nombre";
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

        public List<DocumentoDto> ListadoDocumentos(string rutaDestino) {
            using (var context = new ArchivosEntities()) {
                var listado = context.Documentos
                    .Select(s => new DocumentoDto() {
                        Nombre = s.Nombre
                    }).ToList();
                listado.ForEach(delegate(DocumentoDto dDto) {
                    this.RecuperarDocumento(dDto.Nombre, rutaDestino);
                    string[] archivo = dDto.Nombre.Split('.');
                    dDto.Extension = archivo[archivo.Length - 1];
                });
                return listado;
            }
        }
    }
}
