using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using DataLayer.Reception.Business;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using DataLayer.Data;
using System;
using System.Net.Http.Formatting;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class NationalityController : ApiController
    {
        /// <summary>
        /// Get the list of all nationalities available in HIS.
        /// </summary>
        /// <returns>Return list of all nationalities available in HIS</returns>
        [HttpPost]
        [Route("api/get-nationalities")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);

            NationalityDB _NationalityDB = new NationalityDB();
            List<Nationalities> _allNationalities = _NationalityDB.GetAllNationalities(lang, hospitaId);


            GenericResponse resp = new GenericResponse();

            if (_allNationalities != null && _allNationalities.Count > 0)
            {
                resp.status = 1;
                resp.msg = "Success";
                resp.response = _allNationalities;
                
            }
            else
            {
                resp.status = 0;
                resp.msg = "Fail";
                

            }

            return Ok(resp);
        }

      
    }
}