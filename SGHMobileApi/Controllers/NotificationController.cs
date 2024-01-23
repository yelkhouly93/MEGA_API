using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
	public class NotificationController : ApiController
    {
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };
        private NotificationDB _NotificationDb = new NotificationDB();

		[HttpPost]
        [Route("v2/Notification-PatientPrescription-Get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Get_Notification_PatientPrescription(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) 
                && !string.IsNullOrEmpty(col["patient_reg_no"]) 
                && !string.IsNullOrEmpty(col["VisitID"]) 
                && !string.IsNullOrEmpty(col["DrugID"]) 
                && !string.IsNullOrEmpty(col["PrescriptionId"]) 
                && !string.IsNullOrEmpty(col["Sources"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                int hospitalId = 0;
                int patient_reg_no = 0;
                int VisitID = 0;
                int DrugID = 0;
                int PrescriptionId = 0;
                string Source = "";
                try
                {                    
                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                    VisitID = Convert.ToInt32(col["VisitID"]);
                    DrugID = Convert.ToInt32(col["DrugID"]);
                    PrescriptionId = Convert.ToInt32(col["PrescriptionId"]);
                    Source = col["Sources"].ToString();

                    int errStatus = 0;
                    string errMessage = "";

                    var allDataTable = _NotificationDb.Get_Notification_PatientPrescription_DT(lang,hospitalId,patient_reg_no,VisitID,PrescriptionId,DrugID,Source, ref errStatus, ref errMessage);

                    if (allDataTable.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record Found";
                        _resp.response = allDataTable;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No record Found";
                    }
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }

                
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/Notification-PatientPrescription-add")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Save_Notification_PatientPrescription(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["VisitID"])
                && !string.IsNullOrEmpty(col["DrugID"])
                && !string.IsNullOrEmpty(col["PrescriptionId"])
                && !string.IsNullOrEmpty(col["Sources"])
                && !string.IsNullOrEmpty(col["StartDate"])
                && !string.IsNullOrEmpty(col["StartTimeName"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                int hospitalId = 0;
                int patient_reg_no = 0;
                int VisitID = 0;
                int DrugID = 0;
                int PrescriptionId = 0;
                string Source = "";
                string StartDate = "";
                string StartTimeName = "";

                try
                {
                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                    VisitID = Convert.ToInt32(col["VisitID"]);
                    DrugID = Convert.ToInt32(col["DrugID"]);
                    PrescriptionId = Convert.ToInt32(col["PrescriptionId"]);
                    Source = col["Sources"].ToString();

                    if (!string.IsNullOrEmpty(col["StartDate"]))
                        StartDate = col["StartDate"].ToString();

                    if (!string.IsNullOrEmpty(col["StartTimeName"]))
                        StartTimeName = col["StartTimeName"].ToString();

                    var parsedStartDate = DateTime.Parse(StartDate);
                    DateTime ParsedStartTime;
                    if (!string.IsNullOrEmpty(col["StartTime"]))
                    {
                        string StartTime = col["StartTimeName"].ToString();
                        ParsedStartTime = DateTime.Parse(StartTime);
                    }
                    else
                        ParsedStartTime = parsedStartDate;

                    int errStatus = 0;
                    string errMessage = "";

                    _NotificationDb.SAVE_Notification_PatientPrescription_DT(lang,hospitalId, patient_reg_no, VisitID, PrescriptionId, DrugID, Source, parsedStartDate, ParsedStartTime, StartTimeName, ref errStatus, ref errMessage);

                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/Notification-PatientPrescription-Cancel")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Cancel_Notification_PatientPrescription(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["VisitID"])
                && !string.IsNullOrEmpty(col["DrugID"])
                && !string.IsNullOrEmpty(col["PrescriptionId"])
                && !string.IsNullOrEmpty(col["Sources"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                int hospitalId = 0;
                int patient_reg_no = 0;
                int VisitID = 0;
                int DrugID = 0;
                int PrescriptionId = 0;
                string Source = "";
                try
                {
                    
                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                    VisitID = Convert.ToInt32(col["VisitID"]);
                    DrugID = Convert.ToInt32(col["DrugID"]);
                    PrescriptionId = Convert.ToInt32(col["PrescriptionId"]);
                    Source = col["Sources"].ToString();

                    int errStatus = 0;
                    string errMessage = "";

                    _NotificationDb.Cancel_Notification_PatientPrescription_DT(lang,hospitalId, patient_reg_no, VisitID, PrescriptionId, DrugID, Source, ref errStatus, ref errMessage);

                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }

        [HttpPost]
        [Route("v2/Notification-PatientPrescription-Cancel-All")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Cancel_All_Notification_PatientPrescription(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["patient_reg_no"])                
                && !string.IsNullOrEmpty(col["Sources"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                int hospitalId = 0;
                int patient_reg_no = 0;                
                string Source = "";
                try
                {

                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);                    
                    Source = col["Sources"].ToString();

                    int errStatus = 0;
                    string errMessage = "";

                    _NotificationDb.Cancel_ALL_Notification_PatientPrescription(lang,hospitalId, patient_reg_no, Source, ref errStatus, ref errMessage);

                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }


        //[HttpPost]
        //[Route("v2/Notification-MissedAppointment-Get")]
        ////[ResponseType(typeof(List<GenericResponse>))]
        //[ResponseType(typeof(GenericResponse))]
        //public IHttpActionResult Get_Notification_MissAppointment(FormDataCollection col)
        //{
        //    _resp = new GenericResponse();
        //    _NotificationDb = new NotificationDB();

        //    if (!string.IsNullOrEmpty(col["hospital_id"])
        //        && !string.IsNullOrEmpty(col["patient_reg_no"])
        //        && !string.IsNullOrEmpty(col["Sources"]))
        //    {
        //        var lang = "EN";
        //        if (!string.IsNullOrEmpty(col["lang"]))
        //            lang = col["lang"];


        //        int hospitalId = 0;
        //        int patient_reg_no = 0;                
        //        string Source = "";
        //        try
        //        {
        //            hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);                    
        //            Source = col["Sources"].ToString();

        //            int errStatus = 0;
        //            string errMessage = "";

        //            var allDataTable = _NotificationDb.Get_Notification_MissAppointment_DT(lang, hospitalId, patient_reg_no, Source, ref errStatus, ref errMessage);

        //            if (allDataTable.Rows.Count > 0)
        //            {
        //                _resp.status = 1;
        //                _resp.msg = "Record Found";
        //                _resp.response = allDataTable;
        //            }
        //            else
        //            {
        //                _resp.status = 0;
        //                _resp.msg = "No record Found";
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            _resp.status = 0;
        //            _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
        //            return Ok(_resp);
        //        }


        //    }
        //    else
        //    {
        //        _resp.status = 0;
        //        _resp.msg = "Failed : Missing Parameters";
        //    }
        //    return Ok(_resp);
        //}


        [HttpPost]
        [Route("v2/Notification-PatientSuggestion-Get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Get_Notification_Suggestion(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                int hospitalId = 0;
                int patient_reg_no = 0;
                string Source = "";
                try
                {
                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                    //Source = col["Sources"].ToString();

                    int errStatus = 0;
                    string errMessage = "";

                    var allDataTable = _NotificationDb.Get_Notification_PatientSuggestion_DT(lang, hospitalId, patient_reg_no, Source, ref errStatus, ref errMessage);

                    if (allDataTable.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record Found";
                        _resp.response = allDataTable;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No record Found";
                    }
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }


            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }



        [HttpPost]
        [Route("v2/FCMNotification-Get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        [ResponseType(typeof(GenericResponse))]
        public IHttpActionResult Get_FCMNotification(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _NotificationDb = new NotificationDB();

            if (!string.IsNullOrEmpty(col["TagData"])
                && !string.IsNullOrEmpty(col["TagID"]))
            {

                
                string TagData = "";
                string TagID = "";
                try
                {
                    TagData = col["TagData"].ToString();
                    TagID = col["TagID"].ToString();
                    

                    var allDataTable = _NotificationDb.Get_FCMNotification_DT(TagData,TagID);

                    if (allDataTable.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record Found";
                        _resp.response = allDataTable;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No record Found";
                    }
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }


            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }



    }
}
