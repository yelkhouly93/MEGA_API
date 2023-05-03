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

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientPrescriptionController : ApiController
    {
        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        [HttpPost]
        [Route("api/get-patient-prescription")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
            List<PatientPrescription> _allPatientDiagnosis;
            PatientPrescriptionApiCaller _apiCaller = new PatientPrescriptionApiCaller();

            int errStatus = 0;
            string errMessage = "";
            if (hospitaId != 10)
            {
                PatientDB _patientDB = new PatientDB();
                _allPatientDiagnosis = _patientDB.GetPatientPrescription(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);
            }
            else
            {
                _allPatientDiagnosis = _apiCaller.GetPatientPrescription(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);
            }

            GenericResponse resp = new GenericResponse();

            if (_allPatientDiagnosis != null && _allPatientDiagnosis.Count > 0)
            {
                resp.status = 1;
                resp.msg = errMessage;
                resp.response = _allPatientDiagnosis;
                
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