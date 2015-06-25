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




namespace Archivos.Domain.Managers {
    public class DocumentosManagers {
    

        public MensajeDto guardarDocumento(string nombre,  byte[] contenido ) {
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
    }
}
