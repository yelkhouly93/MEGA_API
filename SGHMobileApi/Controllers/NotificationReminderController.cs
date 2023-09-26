using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGHMobileApi.Common;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Description;






namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class NotificationReminderController : ApiController
    {

        private AppReminder _ReminderDB = new AppReminder();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("v2/Water-Reminder-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostwATERreMINDER(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (col != null)
                {
                    if (!string.IsNullOrEmpty(col["hospital_id"]) 
                        && !string.IsNullOrEmpty(col["Reminder_hour"]) 
                        && !string.IsNullOrEmpty(col["patient_reg_no"]) 
                        && !string.IsNullOrEmpty(col["FormTime"]) 
                        && !string.IsNullOrEmpty(col["ToTime"])
                        && !string.IsNullOrEmpty(col["Sources"])
                        )
                    {

                        var lang = "EN";
                        if (!string.IsNullOrEmpty(col["lang"]))
                            lang = col["lang"].ToString();

                        var hospitaId = Convert.ToInt32(col["hospital_id"]);                        
                        var patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                        var Reminder_hour = Convert.ToInt32(col["Reminder_hour"]);
                        var FormTime = Convert.ToDateTime(col["FormTime"]);
                        var ToTime = Convert.ToDateTime(col["ToTime"]);
                        var Source = col["Sources"];

                        var errStatus = 0;
                        var errMessage = "";

                        _ReminderDB.SaveWaterReminder(lang, hospitaId, patient_reg_no, Reminder_hour , FormTime, ToTime, Source,  ref errStatus, ref errMessage);
                        
                        resp.status = errStatus;
                        resp.msg = errMessage;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "Failed : Missing Parameters";
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/water-reminder-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetWaterReminderList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    //var errStatus = 0;
                    //var errMessage = "";


                    var DTList = _ReminderDB.GetWaterReminder_List(lang, hospitalId, patientMrn);

                    if (DTList != null && DTList.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record Found";
                        resp.response = DTList;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
                    }

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {

                //Log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/water-reminder-Cancel")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult CanceltWaterReminder(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) 
                    && !string.IsNullOrEmpty(col["Sources"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    var source = col["Sources"].ToString();
                    var errStatus = 0;
                    var errMessage = "";


                    _ReminderDB.CancelWaterReminder(lang, hospitalId, patientMrn , source, ref errStatus, ref errMessage);

                    resp.status = 1;
                    resp.msg = errMessage;


                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {

                Log.Error(ex);
            }

            return Ok();
        }



    }
}
