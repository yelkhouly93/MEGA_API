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
using SmartBookingService.Common;
using SGHMobileApi.Common;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class VideoCallConsultationController : ApiController
    {
        /// <summary>
        /// Get Video Consultation Url 
        /// </summary>
        /// <returns>Return URL of video consultation</returns>
        [HttpPost]
        [Route("get-videocall-url")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var selectedDate = Convert.ToDateTime(col["date"]);
            var patientId = Convert.ToInt32(col["patient_id"]);
            var doctorName = col["doctor_name"].ToString();
            var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
            var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());
            var scheduleDayId = Convert.ToInt32(col["schedule_dayId"]);
            
            int errStatus = 0;
            string errMessage = "";

            string generateVideoToken = TokenGenerator.GenerateToken(patientId, timeTo.ToString(), 0);
            string roomKey = Util.GetUniqID();

            string videoUrl = "https://static.vidyo.io/latest/connector/VidyoConnector.html?host=prod.vidyo.io&autoJoin=1&resourceId=" + roomKey + "&displayName=" + doctorName + "&token=" + generateVideoToken;

            PatientDB _patientDb = new PatientDB();
            _patientDb.UpdateVideoCallURL(lang, hospitaId, scheduleDayId, videoUrl, ref errMessage, ref errStatus);


            GenericResponse resp = new GenericResponse();

            if (errStatus == 1)
            {
                resp.status = 1;
                resp.msg = errMessage;
                resp.response = videoUrl;
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