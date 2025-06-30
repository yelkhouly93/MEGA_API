using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Common;
using SGHMobileApi.Extension;
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class Doctors_V3Controller : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GenericResponse _resp = new GenericResponse() { status = 0 };
        private PhysicianDB _physicianDb = new PhysicianDB();
        //private PhysicianDB _physicianDb = new PhysicianDB();

        [HttpPost]
        [Route("v3/doctor-days-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsScheduledDays_v2(FormDataCollection col)
        {
            //!string.IsNullOrEmpty(col["clinic_id"]) &&
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && col["physician_id"] != "0")
                {
                    var lang = col["lang"];
                    var clinicId = 0;
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);

                    var selectedDate = Convert.ToDateTime(System.DateTime.Today.ToString());
                    if (!string.IsNullOrEmpty(col["startdate"]))
                        selectedDate = Convert.ToDateTime(col["startdate"]);

                    var physicianDb = new PhysicianDB();
                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    //for Video Call Selection 
                    var IsVideo = 0;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                        IsVideo = Convert.ToInt32(col["IsVideo"]);

                    if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

                    if (hospitalId == 9)
					{
                        // Call dammam API Function fill list

                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }


                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        var _DataInfo = _loginApiCaller.GetDoctorSchduleDaysByApi_NewDam(lang, physicianId.ToString() , ClinicCode , selectedDate);

                        if (_DataInfo != null && _DataInfo.Count == 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "No record Found";
                            _resp.error_type = "0";

                        }
                        else
                        {

                            _resp.status = 1;
                            _resp.msg = "Record Found";
                            _resp.response = _DataInfo;
                        }

                    }
                    else
					{

                        var allAvailableSlots = physicianDb.GetAvailableDaysOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);

                        if (allAvailableSlots == null || allAvailableSlots.Rows.Count <= 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                        }
                        else
                        {
                            _resp.status = 1;
                            _resp.msg = "Doctor Schedules are available for selected Doctor";
                            _resp.response = allAvailableSlots;
                        }
                    }
                }
                else
                {
                    _resp.msg = "Failed! Missing Parameter";
                }
            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v3/doctor-slots-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsSlots_v2(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) &&
                    col["physician_id"] != "0" && !string.IsNullOrEmpty(col["date"]))
                {

                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var clinicId = Convert.ToInt32(col["clinic_id"]);
                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    //var pageno = Convert.ToInt32(col["page_no"]);
                    //var pagesize = Convert.ToInt32(col["page_size"]);
                    var physicianDb = new PhysicianDB();
                    var availableSlotsApiCaller = new AvailableSlotsApiCaller();
                    List<AvailableSlots> allAvailableSlots;

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }


                    //for Video Call Selection 
                    var IsVideo = 0;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                        IsVideo = Convert.ToInt32(col["IsVideo"]);

                    if (hospitaId==9)
					{
                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }

                        // Call dammam API Function fill list
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        allAvailableSlots = _loginApiCaller.GetDoctorSlotsOfDaysByApi_NewDam(lang, physicianId.ToString(), ClinicCode, selectedDate);


                    }
                    else
					{
                        allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);
                    }


                    

                    //if (!Util.OasisBranches.Contains(hospitaId))
                    //{
                    //    allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //}
                    //else
                    //{
                    //    allAvailableSlots = availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //    //if (pageno > -1)
                    //    //{
                    //    //    allAvailableSlots = allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                    //    //}
                    //}
                    if (allAvailableSlots != null && allAvailableSlots.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Doctor Schedules are available for selected Date";
                        _resp.response = allAvailableSlots;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Doctor Schedules are Not available for selected Date";
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception e)
            {
                _resp.status = 0;
                _resp.msg = e.ToString();
            }

            return Ok(_resp);
        }




        [HttpPost]
        [Route("v3/Mydoctors-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMyDoctorsbyClinic_Copy_v2(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                //var clinicId = Convert.ToInt32(col["clinic_id"]);
                var clinicId = 0;

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

                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize, true, registrationNo);


                if (allPhysician != null && allPhysician.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allPhysician;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
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
        [Route("v4/doctor-days-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsScheduledDays_v4(FormDataCollection col)
        {
            //!string.IsNullOrEmpty(col["clinic_id"]) &&
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && col["physician_id"] != "0")
                {
                    var lang = col["lang"];
                    var clinicId = 0;
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);

                    var selectedDate = Convert.ToDateTime(System.DateTime.Today.ToString());
                    if (!string.IsNullOrEmpty(col["startdate"]))
                        selectedDate = Convert.ToDateTime(col["startdate"]);

                    var physicianDb = new PhysicianDB();
                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    //for Video Call Selection 
                    var IsVideo = 0;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                        IsVideo = Convert.ToInt32(col["IsVideo"]);
                    
                    if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                    {

                        //var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId.ToString(), null, null, physicianId.ToString());
                        
                        //LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var _DataInfo = _UAEApiCaller.GetDoctorSchduleDaysByApi_NewUAE(lang, physicianId.ToString(), hospitalId, selectedDate);

                        if (_DataInfo != null && _DataInfo.Count == 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "No record Found";
                            _resp.error_type = "0";

                        }
                        else
                        {

                            _resp.status = 1;
                            _resp.msg = "Record Found";
                            _resp.response = _DataInfo;
                        }


                    }
                    else if (hospitalId == 9)
                    {
                        // Call dammam API Function fill list

                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }


                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        var _DataInfo = _loginApiCaller.GetDoctorSchduleDaysByApi_NewDam(lang, physicianId.ToString(), ClinicCode, selectedDate);

                        if (_DataInfo != null && _DataInfo.Count == 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "No record Found";
                            _resp.error_type = "0";

                        }
                        else
                        {

                            _resp.status = 1;
                            _resp.msg = "Record Found";
                            _resp.response = _DataInfo;
                        }

                    }
                    else /*for HIS KSA BRANCHES*/
                    {

                        var allAvailableSlots = physicianDb.GetAvailableDaysOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);

                        if (allAvailableSlots == null || allAvailableSlots.Rows.Count <= 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                        }
                        else
                        {
                            _resp.status = 1;
                            _resp.msg = "Doctor Schedules are available for selected Doctor";
                            _resp.response = allAvailableSlots;
                        }


                    }



                }
                else
                {
                    _resp.msg = "Failed! Missing Parameter";
                }
            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v5/doctor-days-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsScheduledDays_v5(FormDataCollection col)
        {
            //!string.IsNullOrEmpty(col["clinic_id"]) &&
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && col["physician_id"] != "0")
                {
                    var lang = col["lang"];
                    var clinicId = 0;
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);

                    var physicianId = Convert.ToInt32(col["physician_id"]);

                    var selectedDate = Convert.ToDateTime(System.DateTime.Today.ToString());
                    if (!string.IsNullOrEmpty(col["startdate"]))
                        selectedDate = Convert.ToDateTime(col["startdate"]);

                    var physicianDb = new PhysicianDB();
                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    //for Video Call Selection 
                    //var IsVideo = 0;
                    //if (!string.IsNullOrEmpty(col["IsVideo"]))
                    //    IsVideo = Convert.ToInt32(col["IsVideo"]);

                    //for Video Call Selection 
                    var IsVideo = 0;
                    var bIsVideo = false;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                    {
                        IsVideo = Convert.ToInt32(col["IsVideo"]);
                        if (IsVideo == 1)
                            bIsVideo = true;
                    }

                    if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                    {
                        //var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId.ToString(), null, null, physicianId.ToString());
                        //LoginApiCaller _loginApiCaller = new LoginApiCaller();

                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var _DataInfo = _UAEApiCaller.GetDoctorSchduleDaysByApi_NewUAE_V5(lang, physicianId.ToString(), hospitalId, selectedDate , bIsVideo);

                        if (_DataInfo != null && _DataInfo.Count == 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "No record Found";
                            _resp.error_type = "0";

                        }
                        else
                        {

                            _resp.status = 1;
                            _resp.msg = "Record Found";
                            _resp.response = _DataInfo;
                        }


                    }
                    else if (hospitalId == 9)
                    {
                        // Call dammam API Function fill list

                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }


                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        var _DataInfo = _loginApiCaller.GetDoctorSchduleDaysByApi_NewDam(lang, physicianId.ToString(), ClinicCode, selectedDate);

                        if (_DataInfo != null && _DataInfo.Count == 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "No record Found";
                            _resp.error_type = "0";

                        }
                        else
                        {

                            _resp.status = 1;
                            _resp.msg = "Record Found";
                            _resp.response = _DataInfo;
                        }

                    }
                    else /*for HIS KSA BRANCHES*/
                    {

                        var allAvailableSlots = physicianDb.GetAvailableDaysOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);

                        if (allAvailableSlots == null || allAvailableSlots.Rows.Count <= 0)
                        {
                            _resp.status = 0;
                            _resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                        }
                        else
                        {
                            _resp.status = 1;
                            _resp.msg = "Doctor Schedules are available for selected Doctor";
                            _resp.response = allAvailableSlots;
                        }


                    }



                }
                else
                {
                    _resp.msg = "Failed! Missing Parameter";
                }
            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v4/doctor-slots-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsSlots_v4(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) &&
                    col["physician_id"] != "0" && !string.IsNullOrEmpty(col["date"]))
                {

                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var clinicId = Convert.ToInt32(col["clinic_id"]);
                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    //var pageno = Convert.ToInt32(col["page_no"]);
                    //var pagesize = Convert.ToInt32(col["page_size"]);
                    var physicianDb = new PhysicianDB();
                    var availableSlotsApiCaller = new AvailableSlotsApiCaller();
                    List<AvailableSlots> allAvailableSlots;

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    //for Video Call Selection 
                    var IsVideo = 0;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                        IsVideo = Convert.ToInt32(col["IsVideo"]);

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
					{   
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        allAvailableSlots = _UAEApiCaller.GetDoctorSlotsOfDaysByApi_NewUAE(lang, physicianId.ToString(), hospitaId, selectedDate);
                    }
                    else if (hospitaId == 9)
                    {
                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }

                        // Call dammam API Function fill list
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        allAvailableSlots = _loginApiCaller.GetDoctorSlotsOfDaysByApi_NewDam(lang, physicianId.ToString(), ClinicCode, selectedDate);


                    }
                    else
                    {
                        allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);
                    }




                    //if (!Util.OasisBranches.Contains(hospitaId))
                    //{
                    //    allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //}
                    //else
                    //{
                    //    allAvailableSlots = availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //    //if (pageno > -1)
                    //    //{
                    //    //    allAvailableSlots = allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                    //    //}
                    //}
                    if (allAvailableSlots != null && allAvailableSlots.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Doctor Schedules are available for selected Date";
                        _resp.response = allAvailableSlots;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Doctor Schedules are Not available for selected Date";
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception e)
            {
                _resp.status = 0;
                _resp.msg = e.ToString();
            }

            return Ok(_resp);
        }

        [HttpPost]
        [Route("v5/doctor-slots-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsSlots_v5(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) &&
                    col["physician_id"] != "0" && !string.IsNullOrEmpty(col["date"]))
                {

                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var clinicId = Convert.ToInt32(col["clinic_id"]);
                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = Convert.ToDateTime(col["date"]);
                    //var pageno = Convert.ToInt32(col["page_no"]);
                    //var pagesize = Convert.ToInt32(col["page_size"]);
                    var physicianDb = new PhysicianDB();
                    var availableSlotsApiCaller = new AvailableSlotsApiCaller();
                    List<AvailableSlots> allAvailableSlots;

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    //for Video Call Selection 
                    var IsVideo = 0;
                    var bIsvideo = false;
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
					{
                        IsVideo = Convert.ToInt32(col["IsVideo"]);
                        if (IsVideo == 1)
                            bIsvideo = true;
                    }   

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        allAvailableSlots = _UAEApiCaller.GetDoctorSlotsOfDaysByApi_NewUAE_V5(lang, physicianId.ToString(), hospitaId, selectedDate , bIsvideo);
                    }
                    else if (hospitaId == 9)
                    {
                        //var ClinicCode = "1390";
                        var ClinicCode = "0";
                        var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitaId.ToString(), null, null, physicianId.ToString());
                        if (allPhysician != null)
                        {
                            if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                            {
                                ClinicCode = allPhysician.DoctorDataList[0].DoctorProfile.ClinicCode.ToString();
                            }
                        }

                        // Call dammam API Function fill list
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        allAvailableSlots = _loginApiCaller.GetDoctorSlotsOfDaysByApi_NewDam(lang, physicianId.ToString(), ClinicCode, selectedDate);


                    }
                    else
                    {
                        allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate, ApiSource, IsVideo);
                    }




                    //if (!Util.OasisBranches.Contains(hospitaId))
                    //{
                    //    allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //}
                    //else
                    //{
                    //    allAvailableSlots = availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    //    //if (pageno > -1)
                    //    //{
                    //    //    allAvailableSlots = allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                    //    //}
                    //}
                    if (allAvailableSlots != null && allAvailableSlots.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Doctor Schedules are available for selected Date";
                        _resp.response = allAvailableSlots;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Doctor Schedules are Not available for selected Date";
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception e)
            {
                _resp.status = 0;
                _resp.msg = e.ToString();
            }

            return Ok(_resp);
        }



        [HttpPost]
        [Route("v4/Mydoctors-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMyDoctorsbyClinic_Copy_v4(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                //var clinicId = Convert.ToInt32(col["clinic_id"]);
                var clinicId = 0;

                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;
                    _resp.msg = "No data found";

                    var strregistrationNo = col["patient_reg_no"].ToString();

                    //_resp.response = allPhysician;
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var returnFormatedStr = _UAEApiCaller.GetMyDoctorSlots_NewUAE(lang, hospitalId, strregistrationNo);


                    var allPhysician = _physicianDb.GetAllPhsiciansDataTable_UAE_MyDoctor(lang, hospitalId, returnFormatedStr, true);
                    if (allPhysician != null && allPhysician.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = allPhysician;

                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No Record Found:";
                    }

                    

                }
				else
				{
                    var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize, true, registrationNo);
                    if (allPhysician != null && allPhysician.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = allPhysician;

                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No Record Found:";
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

    }
}
