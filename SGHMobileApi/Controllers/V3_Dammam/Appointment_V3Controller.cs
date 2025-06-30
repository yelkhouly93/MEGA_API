using AgoraIO.Media;
using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using DataLayer.Data;
using System;
using System.Net.Http.Formatting;
using SmartBookingService.Controllers.ClientApi;
using SGHMobileApi.Common;
using System.Configuration;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class Appointment_V3Controller : ApiController
    {
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PhysicianDB _physicianDb = new PhysicianDB();

        [HttpPost]
        [Route("v3/apt-reservation-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvation_V2(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {

                if (!string.IsNullOrEmpty(col["hospital_id"])
                    && !string.IsNullOrEmpty(col["physician_id"])
                    && !string.IsNullOrEmpty(col["date"])
                    && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["doc_slotID"])
                    && !string.IsNullOrEmpty(col["time_from"])
                    && !string.IsNullOrEmpty(col["time_to"]))
                {
                    var EarlyReminder = 0;
                    var HeardAboutUs = 0;
                    var clinicId = 0;
                    var SlotType = 1;
                    var lang = col["lang"];
                    var sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);


                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

                    if (!string.IsNullOrEmpty(col["Sources"]))
                        sources = col["Sources"];

                    if (!string.IsNullOrEmpty(col["Source"]))
                        sources = col["Source"];

                    if (!string.IsNullOrEmpty(col["SlotType"]))
                        SlotType = Convert.ToInt32(col["SlotType"]);

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);

                    //var test1TimeFrom = (col["time_from"]).ToString();
                    //var test2TimeTO = (col["time_to"]).ToString();

                    //var test2NEWTimeFrom = selectedDate.ToString("yyyy-MM-dd") + " " + test1TimeFrom;

                    //var test2NEWTimeTO = selectedDate.ToString("yyyy-MM-dd") + " " + test2TimeTO;

                    //var NewtimeFrom = Convert.ToDateTime(test2NEWTimeFrom);
                    //var NewtimeTO = Convert.ToDateTime(test2NEWTimeTO);



                    var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
                    var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());


                    var scheduleDayId = Convert.ToInt32(col["doc_slotID"]);
                    var saveAppointmentApiCaller = new SaveAppointmentApiCaller();


                    if (!string.IsNullOrEmpty(col["EarlierReminder"]))
                        EarlyReminder = Convert.ToInt32(col["EarlierReminder"]);

                    if (!string.IsNullOrEmpty(col["HeardAboutUsID"]))
                        HeardAboutUs = Convert.ToInt32(col["HeardAboutUsID"]);

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";

                    var patientDb = new PatientDB();
                    //patientDb.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                    
                    
                    if (hospitaId == 9)
					{
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }
                        
                        var AppoitmentDateTime = selectedDate.ToString("yyyy-MM-dd") + " " + col["time_from"].ToString();
                        isVideoAppointment = 0;

                        LoginApiCaller _loginApiCaller = new LoginApiCaller();

                        // Get Patient Information
                        var IdType = "MRN";
                        var IdValue = patientId.ToString();
                        var userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);

                        PostResponse ReturnObject;

                        var APiResilts = _loginApiCaller.SaveAppointmentApi_NewDammam(physicianId.ToString(), ClinicCode,AppoitmentDateTime , userInfo.national_id, patientId.ToString(),out ReturnObject);
                        if (APiResilts)
						{
                            errStatus = 1;
                            errMessage = "Appoitment Saved Successfully";
                        }
                        else
						{
                            resp.msg = ReturnObject.errorMessage; resp.status = 0;
                            return Ok(resp);
                        }
                            


                        //resp.status = 1;
                        //resp.msg = errMessage;
                        //resp.response = errStatus.ToString();
                        //resp.status = 1;
                        //resp.msg = "Appointment cancelled";
                        //if (lang == "AR" || lang == "ar")
                        //{
                        //    resp.msg = "تم إلغاء الموعد";
                        //}
                    }
                    else
					{
                        patientDb.SaveAppointment_V2(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, sources, SlotType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                    }
                    

                    if (errStatus != 0)
                    {
                        /*
                         * Ahsan New Logic / Changes
                         * if Video Appoitment Booked Succeffully 
                         * Genrate Agora Token  , Channel and Save in Table
                         */
                        if (isVideoAppointment == 1)
                        {
                            //Util.GenerateVideoCallUrl(lang, hospitaId, scheduleDayId, patientId, timeTo.AddHours(2), doctorName, ref errMessage, ref errStatus);
                            var AppoitmentID = errStatus;
                            var Channel = AppoitmentID.ToString();
                            DateTime AppointmentTimeFrom = timeFrom;
                            DateTime AppointmentTimeTo = timeTo;

                            var StartDateTimeTemp = System.DateTime.Now;
                            var ENDtDateTimeTemp = System.DateTime.Now;


                            DateTime StartDateTime = timeFrom.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Agora_StartTime"].ToString()));
                            DateTime EndDateTime = timeTo.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Agora_EndTime"].ToString()));

                            // This is For Test to Call now 
                            //DateTime StartDateTime = System.DateTime.Now;
                            //DateTime EndDateTime = ENDtDateTimeTemp.AddMinutes(Convert.ToInt32("120"));

                            var StartTime = (int)StartDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            var EndTime = (int)EndDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                            var uid = 0;
                            string URLToken = GenerateVideoURL_Token_Test(Channel, StartTime, EndTime, uid);

                            var WebURL = ConfigurationManager.AppSettings["Agora_WebURL"].ToString() + "?SourcesId=DoctorHIS&AppID=" + Channel + "&BranchID=" + hospitaId;
                            var APPID = ConfigurationManager.AppSettings["Agora_APPID"].ToString();
                            var errStatus2 = 0;
                            var errMessage2 = "";
                            patientDb.Save_VideoCallDetails(AppoitmentID.ToString(), hospitaId, Channel, URLToken, StartDateTime, EndDateTime, patientId.ToString(), physicianId, clinicId, WebURL, sources, APPID, AppointmentTimeFrom, AppointmentTimeTo, ref errStatus2, ref errMessage2);
                        }

                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = errStatus.ToString();
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.ToString();
                //Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("v3/apt-reservation-reschedule")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvationReschedule(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {

                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && !string.IsNullOrEmpty(col["date"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["doc_slotID"]) && !string.IsNullOrEmpty(col["time_from"]) && !string.IsNullOrEmpty(col["time_to"]) && !string.IsNullOrEmpty(col["appointment_id"]))
                {
                    var clinicId = 0;
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);


                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);
                    var AppointmentID = Convert.ToInt32(col["appointment_id"]);
                    var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
                    var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());
                    var scheduleDayId = Convert.ToInt32(col["doc_slotID"]);
                    var saveAppointmentApiCaller = new SaveAppointmentApiCaller();

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";
                    var EarlyReminder = 0;
                    var HeardAboutUs = 0;

                    if (!string.IsNullOrEmpty(col["EarlierReminder"]))
                        EarlyReminder = Convert.ToInt32(col["EarlierReminder"]);

                    if (!string.IsNullOrEmpty(col["HeardAboutUsID"]))
                        HeardAboutUs = Convert.ToInt32(col["HeardAboutUsID"]);


                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }


                    var Sources = "";
                    if (!string.IsNullOrEmpty(col["Source"]))
                        Sources = col["Source"];

                    if (!string.IsNullOrEmpty(col["Sources"]))
                        Sources = col["Sources"];

                    var BookType = 1;
                    if (!string.IsNullOrEmpty(col["SlotType"]))
                        BookType = Convert.ToInt32(col["SlotType"]);

                    var patientDb = new PatientDB();
                    try
                    {


                        if (hospitaId == 9)
                        {
                            // New Logic as per Eng. Basmah Approval
                            // Due to Oasis Issue Cancel and Book 
                            if (lang =="EN" || lang == "en"  )
                                resp.msg = "To reschedule the appoitment, Please Book New Appointment and Cancel the old Appointment."; 
                            else
                                resp.msg = "لإعادة جدولة الموعد برجاء حجز موعد جديد وإلغاء الموعد القديم.";


                            resp.status = 0;
                            return Ok(resp);

                            //var ClinicCode = "1390";
                            //var ClinicCode = "0";
                            //var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                            //if (allPhysician != null)
                            //{
                            //    if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            //    {
                            //        ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            //    }
                            //}


                            var AppoitmentDateTime = selectedDate.ToString("yyyy-MM-dd") + " " + col["time_from"].ToString();
                            isVideoAppointment = 0;
                            errStatus = 0;
                            LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            PostResponse ReturnObject;

                            var APiResilts = _loginApiCaller.ReschudleAppointmentApi_NewDammam (AppointmentID.ToString(),  AppoitmentDateTime, out ReturnObject);
                            
                            if (APiResilts)
                                errStatus = 1;
                            else
							{
                                resp.msg = ReturnObject.errorMessage; resp.status = 0;
                                return Ok(resp);
                            }
                        }
                        else
						{
                            patientDb.RescheduleAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, AppointmentID, EarlyReminder, HeardAboutUs, Sources, BookType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                        }



                        


                        if (errStatus != 0)
                        {
                            //if (isVideoAppointment == 1)
                            //{
                            //    Util.GenerateVideoCallUrl(lang, hospitaId, scheduleDayId, patientId, timeTo.AddHours(2), doctorName, ref errMessage, ref errStatus);
                            //}
                            resp.status = 1;
                            //resp.msg = errMessage;
                            //testing
                            resp.msg = "Reschedule Successfully";
                            resp.response = errStatus;
                        }
                        else
                        {
                            resp.status = 0;
                            //resp.msg = errMessage;
                            resp.msg = "Reschedule Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.status = 0;
                        var testtemp = ex.Message.ToString();
                        testtemp = testtemp.Substring(0, Math.Min(testtemp.Length, 700));
                        resp.msg = testtemp;
                    }

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                resp.status = 0;
            }
            return Ok(resp);
        }


        [HttpPost]
        [Route("v3/apt-reservation-cancel")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult CancelAppointment(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Appointment_Id"]))
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                var AppointmentID = Convert.ToInt32(col["Appointment_Id"]);
                int errStatus = 0;
                string errMessage = "";

                var ReasonID = 0;
                ReasonID = Convert.ToInt32(col["Reason_Id"]);

                var Sources = "";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];



                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }


                resp.status = 0;
                resp.msg = "Failed to cancel Appointment. Please try again Later";
                if (lang == "AR")
                {
                    resp.msg = " .لم يتم إلغاء الموعد. الرجاء المحاولة في وقت لاحق ";
                }


                if (hospitaId==9)
				{
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    PostResponse ReturnObject;

                    var APiResilts = _loginApiCaller.CancelAppointmentApi_NewDammam (AppointmentID.ToString() , out ReturnObject) ;

                    if (APiResilts)
					{
                        var patientDb = new PatientDB();
                        patientDb.Save_AppoitmentLogs(hospitaId,AppointmentID.ToString(), col["patient_reg_no"].ToString(), 1, "CANCELLED" , Sources , 0);
                        resp.status = 1;
                        resp.msg = "Appointment cancelled";
                        if (lang == "AR" || lang == "ar")
                        {
                            resp.msg = "تم إلغاء الموعد";
                        }
                    }
                    else
					{
                        resp.msg = ReturnObject.errorMessage; resp.status = 0;
                        return Ok(resp);
                    }
                }
                else
				{
                    var patientDb = new PatientDB();
                    patientDb.CancelAppointment(lang, hospitaId, AppointmentID, registrationNo, ReasonID, Sources, ref errStatus, ref errMessage);
                    if (errStatus != 0)
                    {
                        //var patientDb = new PatientDB();
                        //patientDb.Save_AppoitmentLogs(hospitaId,AppointmentID, col["patient_reg_no"].ToString(), 1, "CANCELLED" , Sources);


                        resp.status = 1;
                        resp.msg = "Appointment cancelled";
                        if (lang == "AR" || lang == "ar")
                        {
                            resp.msg = "تم إلغاء الموعد";
                        }
                    }
                }

                
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed : Missing Parameters";
            }



            return Ok(resp);


        }

        [HttpPost]
        [Route("v3/patient-appointments-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientAppointList(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);



                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                _resp.status = 0;
                _resp.msg = "No Record Found";

                if (hospitalId == 9)
                {                    
                    var apiCaller = new PatientApiCaller();
                    var IdType = "";
                    var IdValue = "";
                    IdType = "MRN";
                    IdValue = registrationNo.ToString();

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var allAppointmnetList = _loginApiCaller.GetPatientAppointmentsByApi_NewDam(lang,IdValue);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }
                }
                else
				{
                    var allAppointmnetList = patientDb.GetPatientAppointmentList(lang, hospitalId, registrationNo, ApiSource);
                    if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }
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
        [Route("v4/patient-appointments-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientAppointList_V4(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                _resp.status = 0;
                _resp.msg = "No Record Found";
                
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
				{
                    var UAEMRN = col["patient_reg_no"].ToString();
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var allAppointmnetList = _UAEApiCaller.GetPatientAppointmentsByApi_NewUAE(lang, UAEMRN.ToString(), hospitalId);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;
                    }
                }
                else if (hospitalId >= 201 && hospitalId < 300)
				{
                    var UAEMRN = col["patient_reg_no"].ToString();
                    ApiCallerEygpt _EYGApiCaller = new ApiCallerEygpt();
                    var allAppointmnetList = _EYGApiCaller.GetPatientAppointmentsByApi_EYGPT(lang, UAEMRN.ToString(), hospitalId);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;
                    }

                }
                else if (hospitalId == 9)
                {
                    var apiCaller = new PatientApiCaller();
                    var IdType = "";
                    var IdValue = "";
                    IdType = "MRN";
                    IdValue = registrationNo.ToString();

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var allAppointmnetList = _loginApiCaller.GetPatientAppointmentsByApi_NewDam(lang, IdValue);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }
                
                }
                else
                {
                    var allAppointmnetList = patientDb.GetPatientAppointmentList(lang, hospitalId, registrationNo, ApiSource);
                    if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }
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
        [Route("v6/patient-appointments-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientAppointList_V6(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                _resp.status = 0;
                _resp.msg = "No Record Found";

                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    var UAEMRN = col["patient_reg_no"].ToString();
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var allAppointmnetList = _UAEApiCaller.GetPatientAppointmentsByApi_NewUAE(lang, UAEMRN.ToString(), hospitalId);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;
                    }
                }
                else if (hospitalId >= 201 && hospitalId < 300)
                {
                    var UAEMRN = col["patient_reg_no"].ToString();
                    ApiCallerEygpt _EYGApiCaller = new ApiCallerEygpt();
                    var allAppointmnetList = _EYGApiCaller.GetPatientAppointmentsByApi_EYGPT(lang, UAEMRN.ToString(), hospitalId);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;
                    }

                }
                else if (hospitalId == 9)
                {
                    var apiCaller = new PatientApiCaller();
                    var IdType = "";
                    var IdValue = "";
                    IdType = "MRN";
                    IdValue = registrationNo.ToString();

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var allAppointmnetList = _loginApiCaller.GetPatientAppointmentsByApi_NewDam(lang, IdValue);
                    if (allAppointmnetList != null && allAppointmnetList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }

                }
                else
                {
                    var allAppointmnetList = patientDb.GetPatientAppointmentList(lang, hospitalId, registrationNo, ApiSource);
                    if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

                    }
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
        [Route("v4/apt-reservation-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvation_V4(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {

                if (!string.IsNullOrEmpty(col["hospital_id"])
                    && !string.IsNullOrEmpty(col["physician_id"])
                    && !string.IsNullOrEmpty(col["date"])
                    && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["doc_slotID"])
                    && !string.IsNullOrEmpty(col["time_from"])
                    && !string.IsNullOrEmpty(col["time_to"]))
                {
                    var EarlyReminder = 0;
                    var HeardAboutUs = 0;
                    var clinicId = 0;
                    var SlotType = 1;
                    var lang = col["lang"];
                    var sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);


                    if (!string.IsNullOrEmpty(col["Sources"]))
                        sources = col["Sources"];

                    if (!string.IsNullOrEmpty(col["Source"]))
                        sources = col["Source"];

                    if (!string.IsNullOrEmpty(col["SlotType"]))
                        SlotType = Convert.ToInt32(col["SlotType"]);

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);

                    //var test1TimeFrom = (col["time_from"]).ToString();
                    //var test2TimeTO = (col["time_to"]).ToString();

                    //var test2NEWTimeFrom = selectedDate.ToString("yyyy-MM-dd") + " " + test1TimeFrom;

                    //var test2NEWTimeTO = selectedDate.ToString("yyyy-MM-dd") + " " + test2TimeTO;

                    //var NewtimeFrom = Convert.ToDateTime(test2NEWTimeFrom);
                    //var NewtimeTO = Convert.ToDateTime(test2NEWTimeTO);



                    var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
                    var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());


                    var scheduleDayId = Convert.ToInt32(col["doc_slotID"]);
                    var saveAppointmentApiCaller = new SaveAppointmentApiCaller();


                    if (!string.IsNullOrEmpty(col["EarlierReminder"]))
                        EarlyReminder = Convert.ToInt32(col["EarlierReminder"]);

                    if (!string.IsNullOrEmpty(col["HeardAboutUsID"]))
                        HeardAboutUs = Convert.ToInt32(col["HeardAboutUsID"]);

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";

                    var patientDb = new PatientDB();
                    //patientDb.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        isVideoAppointment = 0;

                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                        //AppointmentPostResponse
                        //PostResponse ReturnObject;
                        
                        var bookModel = new UAE_BookAppointment ();                        
                        bookModel.AppDate = selectedDate;
                        bookModel.BranchID = hospitaId;
                        bookModel.EndTime = col["time_to"].ToString();
                        bookModel.StartTime= col["time_from"].ToString();
                        bookModel.PatientId = col["patient_reg_no"].ToString();
                        bookModel.PhysicanId = physicianId;

                        AppointmentPostResponse responseOut = new AppointmentPostResponse();

                        var APiResilts = _UAEApiCaller.SaveAppointmentApi_NewUAE(bookModel , out responseOut);
                        if (APiResilts)
                        {
                            var patientDb_N = new PatientDB();
                            patientDb_N.Save_AppoitmentLogs(hospitaId, responseOut.AppId, col["patient_reg_no"].ToString(), 1, "BOOKED", sources , physicianId);


                            resp.msg = "Appoitment Save Successfull"; resp.status = 1;
                            resp.response = responseOut.AppId.ToString();
                        }
                        else
                        {
                            resp.msg = responseOut.Message;  resp.status = 0;
                        }
                        return Ok(resp);

                    }
                    
                    else    if (hospitaId == 9)
                    {
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }

                        var AppoitmentDateTime = selectedDate.ToString("yyyy-MM-dd") + " " + col["time_from"].ToString();
                        isVideoAppointment = 0;

                        LoginApiCaller _loginApiCaller = new LoginApiCaller();

                        // Get Patient Information
                        var IdType = "MRN";
                        var IdValue = patientId.ToString();
                        var userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                        //AppointmentPostResponse
                        PostResponse ReturnObject;

                        var APiResilts = _loginApiCaller.SaveAppointmentApi_NewDammam(physicianId.ToString(), ClinicCode, AppoitmentDateTime, userInfo.national_id, patientId.ToString(), out ReturnObject);
                        if (APiResilts)
                        {
                            var patientDb_N = new PatientDB();
                            patientDb_N.Save_AppoitmentLogs(hospitaId, "1", col["patient_reg_no"].ToString(), 1, "BOOKED", sources, physicianId);


                            errStatus = 1;
                            errMessage = "Appoitment Saved Successfully";
                        }
                        else
                        {
                            resp.msg = ReturnObject.errorMessage; resp.status = 0;
                            return Ok(resp);
                        }



                        //resp.status = 1;
                        //resp.msg = errMessage;
                        //resp.response = errStatus.ToString();
                        //resp.status = 1;
                        //resp.msg = "Appointment cancelled";
                        //if (lang == "AR" || lang == "ar")
                        //{
                        //    resp.msg = "تم إلغاء الموعد";
                        //}
                    }
                    else
                    {
                        patientDb.SaveAppointment_V2(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, sources, SlotType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                    }


                    if (errStatus != 0)
                    {
                        /*
                         * Ahsan New Logic / Changes
                         * if Video Appoitment Booked Succeffully 
                         * Genrate Agora Token  , Channel and Save in Table
                         */
                        if (isVideoAppointment == 1)
                        {
                            //Util.GenerateVideoCallUrl(lang, hospitaId, scheduleDayId, patientId, timeTo.AddHours(2), doctorName, ref errMessage, ref errStatus);
                            var AppoitmentID = errStatus;
                            var Channel = AppoitmentID.ToString();
                            DateTime AppointmentTimeFrom = timeFrom;
                            DateTime AppointmentTimeTo = timeTo;

                            var StartDateTimeTemp = System.DateTime.Now;
                            var ENDtDateTimeTemp = System.DateTime.Now;


                            DateTime StartDateTime = timeFrom.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Agora_StartTime"].ToString()));
                            DateTime EndDateTime = timeTo.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Agora_EndTime"].ToString()));

                            // This is For Test to Call now 
                            //DateTime StartDateTime = System.DateTime.Now;
                            //DateTime EndDateTime = ENDtDateTimeTemp.AddMinutes(Convert.ToInt32("120"));

                            var StartTime = (int)StartDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            var EndTime = (int)EndDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                            var uid = 0;
                            string URLToken = GenerateVideoURL_Token_Test(Channel, StartTime, EndTime, uid);

                            var WebURL = ConfigurationManager.AppSettings["Agora_WebURL"].ToString() + "?SourcesId=DoctorHIS&AppID=" + Channel + "&BranchID=" + hospitaId;
                            var APPID = ConfigurationManager.AppSettings["Agora_APPID"].ToString();
                            var errStatus2 = 0;
                            var errMessage2 = "";
                            patientDb.Save_VideoCallDetails(AppoitmentID.ToString(), hospitaId, Channel, URLToken, StartDateTime, EndDateTime, patientId.ToString(), physicianId, clinicId, WebURL, sources, APPID, AppointmentTimeFrom, AppointmentTimeTo, ref errStatus2, ref errMessage2);
                        }

                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = errStatus.ToString();
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                resp.msg = ex.ToString();
                //Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/apt-reservation-cancel")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult CancelAppointment_V4(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Appointment_Id"]))
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                var AppointmentID = Convert.ToInt32(col["Appointment_Id"]);
                int errStatus = 0;
                string errMessage = "";

                var ReasonID = 0;
                ReasonID = Convert.ToInt32(col["Reason_Id"]);

                var Sources = "";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];

                var DocterID = "";
                var StartTime = "";
                var EndTime = "";
                var AppDate = "";



                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
				{
                    if (string.IsNullOrEmpty(col["App_Date"]) || string.IsNullOrEmpty(col["App_Time"]) || string.IsNullOrEmpty(col["Docter_ID"]))
					{


                        resp.status = 0;
                        resp.msg = "Failed : Missing Parameters";                    
                        return Ok(resp);
                    }
                }
                    if (!string.IsNullOrEmpty(col["App_Date"]))
                    AppDate = col["App_Date"];

                if (!string.IsNullOrEmpty(col["App_Time"]))
                    EndTime = col["App_Time"];

                if (!string.IsNullOrEmpty(col["App_Time"]))
                    StartTime = col["App_Time"];

                var intDoctorID = 0;
                if (!string.IsNullOrEmpty(col["Docter_ID"]))
				{
                    DocterID = col["Docter_ID"];
                    intDoctorID = Convert.ToInt32(col["Docter_ID"]);
                }
                    


                resp.status = 0;
                resp.msg = "Failed to cancel Appointment. Please try again Later";
                if (lang == "AR")
                {
                    resp.msg = " .لم يتم إلغاء الموعد. الرجاء المحاولة في وقت لاحق ";
                }


                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                    var bookModel = new UAE_BookAppointment();
                    
                    bookModel.BranchID = hospitaId;
                    bookModel.PatientId = col["patient_reg_no"].ToString();
                    bookModel.AppId = AppointmentID.ToString();


                    var selectedDate = Convert.ToDateTime(col["date"]);
                    
     //               if (StartTime.Length > 8)
					//{                        
     //                   StartTime = StartTime.Substring(0, 8);
     //               }

                    try
					{
                        bookModel.AppDate = Convert.ToDateTime(AppDate.Replace("%3A", ":"));
                        bookModel.DocId = DocterID;
                        bookModel.PhysicanId = Convert.ToInt32(DocterID);
                        bookModel.StartTime = StartTime.Replace("%3A", ":").ToString();
                        bookModel.EndTime = StartTime.Replace("%3A", ":").ToString();
                    }
                    catch(Exception ex)
					{
                        resp.status = 0;
                        resp.msg = "Failed : Check Parameter Format";
                        return Ok(resp);
                    }

                    

                    
                    

                    AppointmentPostResponse responseOut = new AppointmentPostResponse();

                    var APiResilts = _UAEApiCaller.CancelAppointmentApi_NewUAE(bookModel, out responseOut);
                    if (APiResilts)
                    {
                        var patientDb_N = new PatientDB();
                        patientDb_N.Save_AppoitmentLogs(hospitaId, AppointmentID.ToString(), col["patient_reg_no"].ToString(), 1, "CANCELLED", Sources , intDoctorID);


                        resp.msg = "Appoitment Cancelled Successfully"; resp.status = 1;
                        //resp.response = responseOut.AppId;
                    }
                    else
                    {
                        resp.msg = responseOut.Message; resp.status = 0;
                    }
                    return Ok(resp);

                    //var APiResilts = _UAEApiCaller.CancelAppointmentApi_NewUAE(bookModel);
                    //if (APiResilts)
                    //{
                    //    resp.msg = "Appoitment Cancelled Successfully"; resp.status = 1;
                    //}
                    //else
                    //{
                    //    resp.msg = "Failed | Appoitment Not Cancel"; resp.status = 0;
                    //}
                    //return Ok(resp);

                }
                else if (hospitaId == 9)
                {
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    PostResponse ReturnObject;

                    var APiResilts = _loginApiCaller.CancelAppointmentApi_NewDammam(AppointmentID.ToString(), out ReturnObject);

                    if (APiResilts)
                    {
                        var patientDb_N = new PatientDB();
                        patientDb_N.Save_AppoitmentLogs(hospitaId, AppointmentID.ToString(), col["patient_reg_no"].ToString(), 1, "CANCELLED", Sources, intDoctorID);
                        resp.status = 1;
                        resp.msg = "Appointment cancelled";
                        if (lang == "AR" || lang == "ar")
                        {
                            resp.msg = "تم إلغاء الموعد";
                        }
                    }
                    else
                    {
                        resp.msg = ReturnObject.errorMessage; resp.status = 0;
                        return Ok(resp);
                    }
                }
                else
                {
                    var patientDb = new PatientDB();
                    patientDb.CancelAppointment(lang, hospitaId, AppointmentID, registrationNo, ReasonID, Sources, ref errStatus, ref errMessage);
                    if (errStatus != 0)
                    {
                        resp.status = 1;
                        resp.msg = "Appointment cancelled";
                        if (lang == "AR" || lang == "ar")
                        {
                            resp.msg = "تم إلغاء الموعد";
                        }
                    }
                }


            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed : Missing Parameters";
            }
            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/apt-reservation-reschedule")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvationReschedule_V4(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && !string.IsNullOrEmpty(col["date"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["doc_slotID"]) && !string.IsNullOrEmpty(col["time_from"]) && !string.IsNullOrEmpty(col["time_to"]) && !string.IsNullOrEmpty(col["appointment_id"]))
                {
                    var clinicId = 0;
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);


                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);
                    var AppointmentID = Convert.ToInt32(col["appointment_id"]);
                    var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
                    var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());
                    var scheduleDayId = Convert.ToInt32(col["doc_slotID"]);
                    var saveAppointmentApiCaller = new SaveAppointmentApiCaller();

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";
                    var EarlyReminder = 0;
                    var HeardAboutUs = 0;

                    if (!string.IsNullOrEmpty(col["EarlierReminder"]))
                        EarlyReminder = Convert.ToInt32(col["EarlierReminder"]);

                    if (!string.IsNullOrEmpty(col["HeardAboutUsID"]))
                        HeardAboutUs = Convert.ToInt32(col["HeardAboutUsID"]);



                    var Sources = "";
                    if (!string.IsNullOrEmpty(col["Source"]))
                        Sources = col["Source"];

                    if (!string.IsNullOrEmpty(col["Sources"]))
                        Sources = col["Sources"];

                    var BookType = 1;
                    if (!string.IsNullOrEmpty(col["SlotType"]))
                        BookType = Convert.ToInt32(col["SlotType"]);

                    var patientDb = new PatientDB();
                    try
                    {


                        if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                        {
                            isVideoAppointment = 0;

                            ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                            //AppointmentPostResponse
                            //PostResponse ReturnObject;

                            var bookModel = new UAE_BookAppointment();
                            bookModel.AppDate = selectedDate;
                            bookModel.BranchID = hospitaId;
                            bookModel.EndTime = col["time_to"].ToString();
                            bookModel.StartTime = col["time_from"].ToString();
                            bookModel.PatientId = col["patient_reg_no"].ToString();
                            bookModel.PhysicanId = physicianId;
                            bookModel.AppId = AppointmentID.ToString();


                            AppointmentPostResponse responseOut = new AppointmentPostResponse();

                            var APiResilts = _UAEApiCaller.ReschduleAppointmentApi_NewUAE(bookModel, out responseOut);
                            if (APiResilts)
                            {
                                var patientDb_N = new PatientDB();
                                patientDb_N.Save_AppoitmentLogs(hospitaId, responseOut.AppId.ToString(), col["patient_reg_no"].ToString(), 1, "RESCHEDULED", Sources , physicianId);

                                resp.msg = "Reschedule Successfully"; 
                                resp.status = 1;
                                resp.response = responseOut.AppId;
                            }
                            else
                            {
                                resp.msg = responseOut.Message; 
                                resp.status = 0;
                            }
                            return Ok(resp);



                            //var APiResilts = _UAEApiCaller.ReschduleAppointmentApi_NewUAE(bookModel);
                            //if (APiResilts)
                            //{
                            //    resp.msg = "Appoitment reschdule Successfully"; resp.status = 1;
                            //}
                            //else
                            //{
                            //    resp.msg = "Failed | Appoitment Not reschdule"; resp.status = 0;
                            //}
                            //return Ok(resp);

                        }
                        else if(hospitaId == 9)
                        {
                            // New Logic as per Eng. Basmah Approval
                            // Due to Oasis Issue Cancel and Book 
                            if (lang == "EN" || lang == "en")
                                resp.msg = "To reschedule the appoitment, Please Book New Appointment and Cancel the old Appointment.";
                            else
                                resp.msg = "لإعادة جدولة الموعد برجاء حجز موعد جديد وإلغاء الموعد القديم.";


                            resp.status = 0;
                            return Ok(resp);

                            //var ClinicCode = "1390";
                            //var ClinicCode = "0";
                            //var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                            //if (allPhysician != null)
                            //{
                            //    if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            //    {
                            //        ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            //    }
                            //}


                            var AppoitmentDateTime = selectedDate.ToString("yyyy-MM-dd") + " " + col["time_from"].ToString();
                            isVideoAppointment = 0;
                            errStatus = 0;
                            LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            PostResponse ReturnObject;

                            var APiResilts = _loginApiCaller.ReschudleAppointmentApi_NewDammam(AppointmentID.ToString(), AppoitmentDateTime, out ReturnObject);

                            if (APiResilts)
                                errStatus = 1;
                            else
                            {
                                resp.msg = ReturnObject.errorMessage; resp.status = 0;
                                return Ok(resp);
                            }
                        }
                        else
                        {
                            patientDb.RescheduleAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, AppointmentID, EarlyReminder, HeardAboutUs, Sources, BookType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                        }






                        if (errStatus != 0)
                        {
                            //if (isVideoAppointment == 1)
                            //{
                            //    Util.GenerateVideoCallUrl(lang, hospitaId, scheduleDayId, patientId, timeTo.AddHours(2), doctorName, ref errMessage, ref errStatus);
                            //}
                            resp.status = 1;
                            resp.msg = errMessage;
                            //testing
                            //resp.msg = "Reschedule Successfully";
                            resp.response = errStatus;
                        }
                        else
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            //resp.msg = "Reschedule Failed";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.status = 0;
                        var testtemp = ex.Message.ToString();
                        testtemp = testtemp.Substring(0, Math.Min(testtemp.Length, 700));
                        resp.msg = testtemp;
                    }

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                resp.status = 0;
            }
            return Ok(resp);
        }

        public string GenerateVideoURL_Token_Test(string ChannelName, int startTime, int ExpireTime, long UID)
        {
            //string appID = "0fe4ac748cd24a9fb3ba4730ea201df9";
            //string appCertificate = "3a313f24a1f243d19be7ac04ef3856af";
            //int ts = 1645689245;
            //int r = 123456789;
            //long uid = 0;
            //int expiredTs = 1645775645;

            string appID = ConfigurationManager.AppSettings["Agora_APPID"].ToString();
            string appCertificate = ConfigurationManager.AppSettings["Agora_APPCertificate"].ToString();

            int ts = startTime;
            int r = Convert.ToInt32(ChannelName);
            long uid = UID;
            int expiredTs = ExpireTime;
            uint UexpiredTs = (uint)ExpireTime;

            // AHSAN NEW TESTING
            //string Token = DynamicKey3.generate(appID, appCertificate, ChannelName, ts, r, uid, expiredTs);


            AccessToken token = new AccessToken(appID, appCertificate, ChannelName, "0");

            token.addPrivilege(Privileges.kJoinChannel, UexpiredTs);
            token.addPrivilege(Privileges.kPublishAudioStream, UexpiredTs);
            token.addPrivilege(Privileges.kPublishVideoStream, UexpiredTs);
            string tokKen = token.build();
            var Token = tokKen;


            return Token;
        }

    }
}
