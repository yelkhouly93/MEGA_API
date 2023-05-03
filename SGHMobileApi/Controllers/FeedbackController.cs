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
using System.Globalization;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class FeedbackController : ApiController
    {
        /// <summary>
        /// The patient can submit feedback and rating (from 0 to 5) against his/her visit.
        /// </summary>
        /// <returns>Return the fail/success after saving the rating againt visit</returns>
        [HttpPost]
        [Route("api/reservation-feedback")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientId = col["patient_id"];
            var reservationId = col["reservation_id"];
            var text = col["text"];
            var rating = float.Parse(col["rate"], CultureInfo.InvariantCulture.NumberFormat);
            
            int status = 0;
            string msg = "";
            

            FeedbackDB _feedbackDB = new FeedbackDB();
            _feedbackDB.SavePatientFeedback(lang, hospitalId, Convert.ToInt32(patientId), Convert.ToInt32(reservationId), text, rating, ref status, ref msg);

            GenericResponse resp = new GenericResponse();

            if (status > 0)
            {
                resp.msg = msg;
                resp.response = "";
                resp.status = 1;
            }
            else
            {
                resp.msg = msg;
                //resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }
    }
}


