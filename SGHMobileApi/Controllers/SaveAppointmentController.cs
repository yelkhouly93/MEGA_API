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

// ReSharper disable once CheckNamespace
namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class SaveAppointmentController : ApiController
    {
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Save appointment against patient
        /// </summary>
        /// <returns>Return status of Save appointment</returns>
        [HttpPost]
        [Route("apt-reservation-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvation(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {

                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["clinic_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && !string.IsNullOrEmpty(col["date"]) && !string.IsNullOrEmpty(col["patient_id"]) && !string.IsNullOrEmpty(col["schedule_dayId"]) && !string.IsNullOrEmpty(col["time_from"]) && !string.IsNullOrEmpty(col["time_to"]))
                {
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var clinicId = Convert.ToInt32(col["clinic_id"]);
                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    var patientId = Convert.ToInt32(col["patient_id"]);
                    var timeFrom = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_from"]).ToString());
                    var timeTo = Convert.ToDateTime(selectedDate.ToString("yyyy-MM-dd") + " " + (col["time_to"]).ToString());
                    var scheduleDayId = Convert.ToInt32(col["schedule_dayId"]);
                    var saveAppointmentApiCaller = new SaveAppointmentApiCaller();

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";
                    var heardAboutUS = 0;

                    if (!Util.OasisBranches.Contains(hospitaId))
                    {
                        var patientDb = new PatientDB();
                        patientDb.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId,0, heardAboutUS, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);

                    }
                    else
                    {
                        saveAppointmentApiCaller.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, ref errStatus, ref errMessage);
                    }
                    if (errStatus != 0)
                    {
                        if (isVideoAppointment == 1)
                        {
                            Util.GenerateVideoCallUrl(lang, hospitaId, scheduleDayId, patientId, timeTo.AddHours(2), doctorName, ref errMessage, ref errStatus);
                        }
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = errStatus;
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
                Log.Error(ex);
                resp.status = 0;
            }
            

            
            
            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/apt-reservation-add")]
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

                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }
                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

                    //for SALE FORCE POINT - for REPORTING 
                    var agent_Userid = "";
                    var agent_UserName = "";

                    if (!string.IsNullOrEmpty(col["agent_Userid"]))
                        agent_Userid = col["agent_Userid"];
                    if (!string.IsNullOrEmpty(col["agent_UserName"]))
                        agent_UserName = col["agent_UserName"];
                    //for SALE FORCE POINT - for REPORTING 


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
                    patientDb.SaveAppointment_V2(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, sources, SlotType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName , agent_UserName,agent_Userid);
                    




                    // No Need this Code in Version 2 
                    //if (!Util.OasisBranches.Contains(hospitaId))
                    //{
                    //    var patientDb = new PatientDB();
                    //    patientDb.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, EarlyReminder, HeardAboutUs, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);

                    //}
                    //else
                    //{
                    //    saveAppointmentApiCaller.SaveAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, ref errStatus, ref errMessage);
                    //}

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
                            patientDb.Save_VideoCallDetails(AppoitmentID.ToString(), hospitaId, Channel, URLToken, StartDateTime, EndDateTime, patientId.ToString(), physicianId, clinicId, WebURL, sources, APPID, AppointmentTimeFrom , AppointmentTimeTo, ref errStatus2, ref errMessage2);                        }

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
                Log.Error(ex);
                resp.status = 0;
            }




            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/apt-Nearest-reservation-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppointmentResrvationByNearest_V2(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {

                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"])  && !string.IsNullOrEmpty(col["patient_reg_no"]) )
                {
                    var EarlyReminder = 0;
                    var HeardAboutUs = 0;
                    var clinicId = 0;
                    var lang = col["lang"];                    
                    var sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

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


                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);                
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);

                    if (!string.IsNullOrEmpty(col["EarlierReminder"]))
                        EarlyReminder = Convert.ToInt32(col["EarlierReminder"]);

                    if (!string.IsNullOrEmpty(col["HeardAboutUsID"]))
                        HeardAboutUs = Convert.ToInt32(col["HeardAboutUsID"]);

                    var errStatus = 0;
                    var errMessage = "";
                    var isVideoAppointment = 0;
                    var doctorName = "";

                    var patientDb = new PatientDB();                    
                    patientDb.SaveAppointment_Nearest_V2(lang, hospitaId, clinicId, physicianId, System.DateTime.Now, patientId, System.DateTime.Now, System.DateTime.Now, 0, EarlyReminder, HeardAboutUs, sources, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName);
                    
                    if (errStatus != 0)
                    {   
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = errStatus;
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
                Log.Error(ex);
                resp.status = 0;
            }




            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/apt-reservation-reschedule")]
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
                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

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

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

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


                    //for SALE FORCE POINT - for REPORTING 
                    var agent_Userid = "";
                    var agent_UserName = "";

                    if (!string.IsNullOrEmpty(col["agent_Userid"]))
                        agent_Userid = col["agent_Userid"];
                    if (!string.IsNullOrEmpty(col["agent_UserName"]))
                        agent_UserName = col["agent_UserName"];
                    //for SALE FORCE POINT - for REPORTING 



                    var patientDb = new PatientDB();
                    try
                    {
                        patientDb.RescheduleAppointment(lang, hospitaId, clinicId, physicianId, selectedDate, patientId, timeFrom, timeTo, scheduleDayId, AppointmentID, EarlyReminder, HeardAboutUs, Sources, BookType, ref errStatus, ref errMessage, ref isVideoAppointment, ref doctorName , agent_UserName , agent_Userid);


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
                Log.Error(ex);
                resp.status = 0;
            }




            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/apt-reservation-cancel")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult CancelAppointment(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Appointment_Id"]) )
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                var AppointmentID = Convert.ToInt32(col["Appointment_Id"]);
                int errStatus = 0;
                string errMessage = "";

                var ReasonID= 0;
                ReasonID = Convert.ToInt32(col["Reason_Id"]);

                var Sources = "";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];

                //for SALE FORCE POINT - for REPORTING 
                var agent_Userid = "";
                var agent_UserName = "";

                if (!string.IsNullOrEmpty(col["agent_Userid"]))
                    agent_Userid = col["agent_Userid"];
                if (!string.IsNullOrEmpty(col["agent_UserName"]))
                    agent_UserName = col["agent_UserName"];
                //for SALE FORCE POINT - for REPORTING 


                var patientDb = new PatientDB();
                patientDb.CancelAppointment(lang, hospitaId, AppointmentID, registrationNo, ReasonID, Sources, ref errStatus, ref errMessage , agent_Userid , agent_UserName);

                if (errStatus != 0 )
                {
                    resp.status = 1;
                    //resp.msg = "Appointment cancelled  \n\n  ! تم إلغاء الموعد";
                    resp.msg = "Appointment cancelled";
                    if (lang =="AR" || lang == "ar")
                    {
                        resp.msg = "تم إلغاء الموعد";
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed to cancel Appointment. Please try again Later";
                    if (lang == "AR")
                    {
                        resp.msg = " .لم يتم إلغاء الموعد. الرجاء المحاولة في وقت لاحق ";
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
        [Route("v2/apt-reservation-confirm")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult confirmAppointment(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Appointment_Id"]) && !string.IsNullOrEmpty(col["Sources"]) )
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                var AppointmentID = Convert.ToInt32(col["Appointment_Id"]);
                int errStatus = 0;
                string errMessage = "";

                var Sources = "";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];

                var patientDb = new PatientDB();
                patientDb.ConfirmAppointment(lang, hospitaId, AppointmentID, registrationNo, Sources,  ref errStatus, ref errMessage);

                if (errStatus != 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;                    
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



            return Ok(resp);


        }





        [HttpPost]
        [Route("v2/patient-appointments-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientAppointList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                if (hospitalId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
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

                var allAppointmnetList = patientDb.GetPatientAppointmentList(lang, hospitalId, registrationNo , ApiSource);


                if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allAppointmnetList;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
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
        [Route("v2/patient-Cancelled-appointments-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientCancelledApointmentList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                if (hospitalId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var allAppointmnetList = patientDb.GetPatientCancelledAppointmentList(lang, hospitalId, registrationNo);


                if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allAppointmnetList;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        // Mainly Developed FOR SaleForces
        [HttpPost]
        [Route("v2/all-appointment-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult UpCommingAppointment(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";
            int ResgistrationID = 0;
            if (!string.IsNullOrEmpty(col["hospital_id"]))
            {
                //!string.IsNullOrEmpty(col["FromDate"]) && !string.IsNullOrEmpty(col["ToDate"])
                DateTime ToDate = Convert.ToDateTime ("1/1/1900");
                DateTime FromDate = Convert.ToDateTime("1/1/1900");

                if (!string.IsNullOrEmpty(col["ToDate"]))
                    ToDate  = Convert.ToDateTime(col["ToDate"]);
                
                if (!string.IsNullOrEmpty(col["FromDate"]))
                    FromDate = Convert.ToDateTime(col["FromDate"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    ResgistrationID = Convert.ToInt32(col["patient_reg_no"]);


                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var patientDb = new PatientDB();
                var dtTable = patientDb.GetAllAppointmentList(lang, hospitaId, ToDate,FromDate, ResgistrationID);

                if (dtTable != null && dtTable.Rows.Count > 0 )
                {
                    resp.status = 1;                    
                    resp.msg = "Records Found";
                    resp.response = dtTable;


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
                resp.msg = "Failed : Missing Parameters";
            }



            return Ok(resp);


        }


        // Mainly Developed FOR Genesys
        [HttpPost]
        [Route("v2/Future-appointments-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult FutureAppointment(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";
            int ResgistrationID = 0;
            if (!string.IsNullOrEmpty(col["hospital_id"]))
            {
                //!string.IsNullOrEmpty(col["FromDate"]) && !string.IsNullOrEmpty(col["ToDate"])
                DateTime ToDate = Convert.ToDateTime("1/1/1900");
                DateTime FromDate = Convert.ToDateTime("1/1/1900");

                if (!string.IsNullOrEmpty(col["ToDate"]))
                    ToDate = Convert.ToDateTime(col["ToDate"]);

                if (!string.IsNullOrEmpty(col["FromDate"]))
                    FromDate = Convert.ToDateTime(col["FromDate"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    ResgistrationID = Convert.ToInt32(col["patient_reg_no"]);


                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }


                var patientDb = new PatientDB();
                var dtTable = patientDb.GetAllAppointmentList(lang, hospitaId, ToDate, FromDate, ResgistrationID);

                if (dtTable != null && dtTable.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Records Found";
                    resp.response = dtTable;


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
                resp.msg = "Failed : Missing Parameters";
            }



            return Ok(resp);


        }


        [HttpPost]
        [Route("v2/VideoCallDetails-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GETVedioCallDetails(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();            
            var lang = "EN";

                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Appointment_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var Appointmentid = col["Appointment_id"].ToString();
                    var patient_reg_no = col["patient_reg_no"].ToString();
                    var errStatus = 0;
                    var errMessage = "";

                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;                    
                    _resp.msg = "Sorry this service not available";
                    
                    return Ok(_resp);
                }

                if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var patientDb = new PatientDB();
                    var ReturnResponse  = patientDb.GET_VideoCallDetails(lang , Appointmentid, hospitaId, patient_reg_no,ref errStatus, ref errMessage);

                    if (errStatus != 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = ReturnResponse;
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

            return Ok(resp);
        }


        [HttpPost]
        [Route("v3/VideoCallDetails-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GETVedioCallDetails_V3(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            var lang = "EN";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Appointment_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var Appointmentid = col["Appointment_id"].ToString();
                var patient_reg_no = col["patient_reg_no"].ToString();
                var errStatus = 0;
                var errMessage = "";
                var patientDb = new PatientDB();
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    //_resp.status = 0;
                    //_resp.msg = "Sorry this service not available";
                    var ReturnResponseUAE = patientDb.GET_VideoCallDetails_V3(lang, Appointmentid, hospitaId, patient_reg_no, ref errStatus, ref errMessage);

                    if (errStatus != 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = ReturnResponseUAE;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                    return Ok(_resp);
                }

                

                
                var ReturnResponse = patientDb.GET_VideoCallDetails(lang, Appointmentid, hospitaId, patient_reg_no, ref errStatus, ref errMessage);

                if (errStatus != 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.response = ReturnResponse;
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

            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/VideoCallPatientJoin-Update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult UpdateVideoCallDetails_PatientJoin(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            //var lang = "EN";
            var JoinBy = "Patient";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Appointment_id"]))
            {
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var Appointmentid = col["Appointment_id"].ToString();
                //var patient_reg_no = col["patient_reg_no"].ToString();
                var errStatus = 0;
                var errMessage = "";

                //if (!string.IsNullOrEmpty(col["lang"]))
                //    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["JoinBy"]))
                    JoinBy = col["JoinBy"];

                var patientDb = new PatientDB();
                patientDb.UPDATE_VideoCallPatientJoin(Appointmentid, hospitaId, JoinBy);

                resp.status = 1;
                resp.msg = "Record has been Updated.";                
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed : Missing Parameters";
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("v2/VideoCall-Postponed-byDoctor")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult VideoCall_Postponed_byDoctor(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (
                !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Doctor_Name"])
                && !string.IsNullOrEmpty(col["Appointment_id"]) && !string.IsNullOrEmpty(col["Department_Name"])
                && !string.IsNullOrEmpty(col["Patient_MRN"]) && !string.IsNullOrEmpty(col["Appointment_Date"])
                && !string.IsNullOrEmpty(col["postponed_min"]) && !string.IsNullOrEmpty(col["Source"])
                && !string.IsNullOrEmpty(col["Doctor_ID"])
                )
            {
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                if (hospitaId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var Appointmentid = Convert.ToInt32(col["Appointment_id"]);
                var PatientMRN = Convert.ToInt32(col["Patient_MRN"]);
                var postponedmin = Convert.ToInt32(col["postponed_min"]);
                var DoctorId = Convert.ToInt32(col["Doctor_ID"]);

                var DoctorName = col["Doctor_Name"].ToString();
                var DepartmentName = col["Department_Name"].ToString();
                var AppointmentDate = col["Appointment_Date"].ToString();
                var Source = col["Source"].ToString();

                var errStatus = 0;
                var errMessage = "";

                var patientDb = new PatientDB();
                patientDb.Save_VideoCall_Postponed_byDoctor(hospitaId, Appointmentid, PatientMRN, postponedmin, DoctorId, DoctorName, DepartmentName, AppointmentDate, Source, ref errStatus, ref errMessage);

                resp.status = errStatus;
                //resp.msg = "Record has been Updated.";
                resp.msg = errMessage;
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed : Missing Parameters";
            }

            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/Doctor-VideoCall-Join-Msg-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctor_JoinStatus_Message(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Appointment_ID"])  && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                if (hospitalId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                var AppoitmentID = Convert.ToInt32(col["Appointment_ID"]);



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Source"]))
                    ApiSource = col["Source"].ToString();

                var DataTableReturn = patientDb.GetVidoCallDoctorStatusMsg(lang, hospitalId, registrationNo, AppoitmentID , ApiSource);


                if (DataTableReturn != null && DataTableReturn.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = DataTableReturn;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

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
            uint UexpiredTs = (uint) ExpireTime;

            // AHSAN NEW TESTING
            //string Token = DynamicKey3.generate(appID, appCertificate, ChannelName, ts, r, uid, expiredTs);


            AccessToken token = new AccessToken (appID, appCertificate, ChannelName, "0");
            
            token.addPrivilege(Privileges.kJoinChannel , UexpiredTs);
            token.addPrivilege(Privileges.kPublishAudioStream, UexpiredTs);
            token.addPrivilege(Privileges.kPublishVideoStream , UexpiredTs);
            string tokKen = token.build();
            var Token = tokKen;


            return Token;
        }



        [HttpPost]
        [Route("v2/patient-missed-appointments-list-Notification")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientMissedApointmentList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                if (hospitalId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var StrpatientMrn = col["patient_reg_no"];
                    var PatientData = _UAEApiCaller.GetPatientMissedAppoitmentByApi_UAE(lang, hospitalId, StrpatientMrn.ToString());
                    
                    if (PatientData != null && PatientData.Count > 0)
                    {
                        var returnData = patientDb.GetPatientMissedAppointmentList_UAE(lang, hospitalId, registrationNo, PatientData);

                        if (returnData != null && returnData.Rows.Count > 0)
						{
                            _resp.status = 1;
                            _resp.msg = "Record(s) Found";
                            _resp.response = returnData;
                        }
                    }


                    return Ok(_resp);

                }
                else
				{
                    _resp.status = 0;
                    if (lang == "EN")
                        _resp.msg = "Sorry this service not available";
                    else
                        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);

                }

                _resp.status = 0;
                _resp.msg = "No Record Found";

                // KSA NOT not Implemented
                //var allAppointmnetList = patientDb.GetPatientMissedAppointmentList(lang, hospitalId, registrationNo);
                //if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                //{
                //    _resp.status = 1;
                //    _resp.msg = "Record(s) Found";
                //    _resp.response = allAppointmnetList;

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "No Record Found";
                //}

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