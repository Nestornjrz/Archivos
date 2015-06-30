using Archivos.Application.Dto;
using Archivos.Domain.Db;
using Archivos.Domain.Managers.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivos.Domain.Managers {
    public class LugaresManagers {
        public List<LugareDto> ListadoLugares() {
            using (var context = new ArchivosEntities()) {
                var listado = context.Lugares
                    .Select(s => new LugareDto() {
                        LugarID = s.LugarID,
                        NombreLugar = s.NombreLugar
                    }).ToList();
                return listado;
            }
        }

        public MensajeDto CargarLugar(LugareDto lDto) {
            if (lDto.LugarID > 0) {
                return EditarLugar(lDto);
            }
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var lugareDb = new Lugare();                
                lugareDb.NombreLugar = lDto.NombreLugar;

                context.Lugares.Add(lugareDb);

                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }
                lDto.LugarID = lugareDb.LugarID;

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se cargo el lugar : " + lDto.LugarID,
                    ObjetoDto = lDto
                };
            }
        }

        private MensajeDto EditarLugar(LugareDto lDto) {
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var lugareDb = context.Lugares
                    .Where(l => l.LugarID == lDto.LugarID).FirstOrDefault();
                if (lugareDb == null) {
                     return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "No existe el lugar con id " + lDto.LugarID
                    };
                }
                lugareDb.NombreLugar = lDto.NombreLugar;
                context.Entry(lugareDb).State = System.Data.Entity.EntityState.Modified;
                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se Edito el lugar : " + lDto.LugarID,
                    ObjetoDto = lDto
                };
            }
        }

        public MensajeDto EliminarLugar(int id) {
            using (var context = new ArchivosEntities()) {
                MensajeDto mensajeDto = null;
                var lugareDb = context.Lugares
                    .Where(l => l.LugarID == id).FirstOrDefault();
                if (lugareDb == null) {
                    return new MensajeDto() {
                        Error = true,
                        MensajeDelProceso = "No existe el lugar con id " + id
                    };
                }

                context.Lugares.Remove(lugareDb);

                mensajeDto = AgregarModificar.Hacer(context, mensajeDto);
                if (mensajeDto != null) { return mensajeDto; }

                return new MensajeDto() {
                    Error = false,
                    MensajeDelProceso = "Se elimino el lugar : " + id
                };
            }
        }
    }
}
