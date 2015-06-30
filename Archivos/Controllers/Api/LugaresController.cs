using Archivos.Application.Dto;
using Archivos.Domain.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Archivos.Controllers.Api
{
    public class LugaresController : ApiController
    {
        // GET: api/Lugares
        public HttpResponseMessage Get()
        {
            LugaresManagers lm = new LugaresManagers();
            List<LugarDto> listado = lm.ListadoLugares();
            return Request.CreateResponse<List<LugarDto>>(HttpStatusCode.OK, listado);
        }

        // GET: api/Lugares/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Lugares
        public HttpResponseMessage Post(LugarDto lDto)
        {
            LugaresManagers lm = new LugaresManagers();
            MensajeDto mensaje = lm.CargarLugar(lDto);
            return Request.CreateResponse(HttpStatusCode.Created, mensaje);
        }

        // PUT: api/Lugares/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Lugares/5
        public HttpResponseMessage Delete(int id)
        {
            LugaresManagers lm = new LugaresManagers();
            MensajeDto mensaje = lm.EliminarLugar(id);
            return Request.CreateResponse(HttpStatusCode.Created, mensaje);
        }
    }
}
