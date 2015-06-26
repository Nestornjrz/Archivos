using Archivos.Application.Dto;
using Archivos.Domain.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Archivos.Controllers.Api {
    public class DocumentosController : ApiController {
        // GET: api/Documentos
        public HttpResponseMessage Get() {
            DocumentosManagers dm = new DocumentosManagers();
            string ruta = Path.Combine("~/images/docs");
            var rutaDestino = HttpContext.Current.Server.MapPath(ruta);
            List<DocumentoDto> listado = dm.ListadoDocumentos(rutaDestino);
            return Request.CreateResponse<List<DocumentoDto>>(HttpStatusCode.OK, listado);
        }

        // GET: api/Documentos/5
        public string Get(int id) {
            return "value";
        }

        // POST: api/Documentos
        public HttpResponseMessage Post() {
            DocumentosManagers dm = new DocumentosManagers();
            MensajeDto mensaje = null;
            List<MensajeDto> listadoMensajeArchivos = new List<MensajeDto>();
            var request = HttpContext.Current.Request;
            //Se recupera las variables enviadas desde el formulario 
            var titulo = request["titulo"];
            var descripcion = request["descripcion"];

            if (request.Files.Count > 0) {
                var cantidadArchivosSinError = request.Files.Count;
                foreach (string file in request.Files) {
                    var postedFile = request.Files[file];
                    using (var binaryReader = new BinaryReader(postedFile.InputStream)) {
                        byte[] fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                        var mensajeCadaUno = dm.guardarDocumento(postedFile.FileName, fileData);
                        if (mensajeCadaUno.Error) { cantidadArchivosSinError -= 1; }
                        listadoMensajeArchivos.Add(mensajeCadaUno);
                    }
                }
                string logArchivos = "Ningun mensaje cargado";
                bool errorCompleto = false;
                if (request.Files.Count == 1) {
                    logArchivos = "Archivo cargado";
                }else if (cantidadArchivosSinError == request.Files.Count) {
                    logArchivos = "Todos los archivos cargados";
                } else if (cantidadArchivosSinError > 0) {
                    logArchivos = "Algunos archivos cargados";
                } else {
                    logArchivos = "Algunos archivos cargados";
                    errorCompleto = true;
                }
                mensaje = new MensajeDto() {
                    Error = errorCompleto,
                    MensajeDelProceso = logArchivos,
                    ObjetoDto = listadoMensajeArchivos,
                    Valor = cantidadArchivosSinError.ToString()
                };
                return Request.CreateResponse(HttpStatusCode.Created, mensaje);
            } else {
                mensaje = new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "No se envio ningun archivo"
                };
                return Request.CreateResponse(HttpStatusCode.BadRequest, mensaje);
            }

        }

        // PUT: api/Documentos/5
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE: api/Documentos/5
        public void Delete(int id) {
        }
    }
}
