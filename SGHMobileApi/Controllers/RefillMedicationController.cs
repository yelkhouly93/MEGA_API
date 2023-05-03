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
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class RefillMedicationController : ApiController
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Save Refill Medication Request.
        /// </summary>
        /// <returns>Return availabke slots against physician and appointment date</returns>
        [HttpPost]
        [Route("save-refill-medication")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {


            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var registrationNo = Convert.ToInt32(col["registration_no"]);
            var iqamaPassportNo = col["iqama_passport_no"];
            var contactNumber = col["contact_number"];
            var selectedSpeciality = col["selected_speciality"];
            var sourceEntryId = Convert.ToInt32(col["source_entry_id"]);

            int errStatus = 0;
            string errMessage = "";

            if (hospitaId != 10)
            {
                PatientDB _patientDB = new PatientDB();
                _patientDB.SaveRefillMedication(lang, hospitaId, registrationNo, iqamaPassportNo,contactNumber, selectedSpeciality, sourceEntryId, ref errStatus, ref errMessage);
            }
            else
            {
                
            }

            GenericResponse resp = new GenericResponse();

            if (errStatus == 1)
            {
                resp.status = 1;
                resp.msg = errMessage;
            }
            else
            {
                resp.status = 0;
                resp.msg = errMessage;
            }

            return Ok(resp);
        }
	}
}