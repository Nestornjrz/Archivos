using Archivos.Application.Dto;
using Archivos.Domain.Db;
using Archivos.Domain.Managers.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivos.Domain.Managers {
    public class ArchivosMovimientosCabsManagers {
        public List<ArchivosMovimientoCabDto> ListadoArchivosMovimientoCab() {
            using (var context = new ArchivosEntities()) {
                var listado = context.ArchivosMovimientosCabs
                    .Select(s => new ArchivosMovimientoCabDto() {
                        ArchivosMovimientoCabID = s.ArchivosMovimientoCabID,
                        Titulo = s.Titulo
                    })
                    .ToList();
                return listado;
            }
        }

        public MensajeDto CargarArchivoMovimientoCab(ArchivosMovimientoCabDto amcDto) {
            if (amcDto.ArchivosMovimientoCabID > 0) {
                return EditarArchivoMovimientoCab(amcDto);
            }
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var archivosMovimientosCabDB = new ArchivosMovimientosCab();
                archivosMovimientosCabDB.Titulo = amcDto.Titulo;

                context.ArchivosMovimientosCabs.Add(archivosMovimientosCabDB);

                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }
                amcDto.ArchivosMovimientoCabID = archivosMovimientosCabDB.ArchivosMovimientoCabID;

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se cargo el ArchivosMovimientosCabs : " + amcDto.ArchivosMovimientoCabID,
                    ObjetoDto = amcDto
                };
            }
        }

        private MensajeDto EditarArchivoMovimientoCab(ArchivosMovimientoCabDto amcDto) {
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var archivosMovimientosCabDB = context.ArchivosMovimientosCabs
                    .Where(a => a.ArchivosMovimientoCabID == amcDto.ArchivosMovimientoCabID)
                    .FirstOrDefault();
                if (archivosMovimientosCabDB == null) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "No existe el ArchivosMovimientosCab con id " + amcDto.ArchivosMovimientoCabID
                    };
                }

                archivosMovimientosCabDB.Titulo = amcDto.Titulo;

                context.Entry(archivosMovimientosCabDB).State = System.Data.Entity.EntityState.Modified;
                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se Edito el archivosMovimientosCab : " + amcDto.ArchivosMovimientoCabID,
                    ObjetoDto = amcDto
                };
            }
        }

        public MensajeDto EliminarArchivoMovimientoCab(int id) {
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var archivosMovimientosCabDB = context.ArchivosMovimientosCabs
                    .Where(a => a.ArchivosMovimientoCabID == id)
                    .FirstOrDefault();
                if (archivosMovimientosCabDB == null) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "No existe el archivosMovimientosCab con id " + id
                    };
                }

                context.ArchivosMovimientosCabs.Remove(archivosMovimientosCabDB);

                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se elimino el archivosMovimientosCab : " + id
                };
            }
        }
    }
}
