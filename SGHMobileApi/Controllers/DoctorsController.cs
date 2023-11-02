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
#pragma warning disable 1591
    [Authorize]
    [AuthenticationFilter]
    public class DoctorsController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GenericResponse _resp = new GenericResponse() { status = 0 };
        private PhysicianDB _physicianDb = new PhysicianDB();

        [HttpPost]
        [Route("doctors")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsbyClinic(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])  && !string.IsNullOrEmpty(col["clinic_id"]) && col["clinic_id"] !="0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"] ))
                    lang = col["lang"];

                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var clinicId = Convert.ToInt32(col["clinic_id"]);

                
                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize);

                
                if (allPhysician != null && allPhysician.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allPhysician;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed:";
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
        [Route("doctors-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsbyClinic_Copy(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["clinic_id"]) && col["clinic_id"] != "0")
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
                var clinicId = Convert.ToInt32(col["clinic_id"]);


                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize);


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
        [Route("doctors-all")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsbyHospital(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]))
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


                
                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, 0, pageNo, pageSize);


                if (allPhysician != null && allPhysician.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allPhysician;

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
        [Route("doctor-fee")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostConsultationAmount(FormDataCollection col)

        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientId = Convert.ToInt32(col["patient_id"]);
            var doctorId = Convert.ToInt32(col["doctor_id"]);
            var doctorScheduleId = Convert.ToInt32(col["doctor_schedule_id"]);
            var billType = col["bill_type"];
            //ConsultationAmount consultationAmount = null;
            

            OnlinePaymentApiCaller apiCaller = new OnlinePaymentApiCaller();
            
            _physicianDb = new PhysicianDB();


            int errStatus = 0;
            string errMessage = "";




            _resp = new GenericResponse {status = 0, msg = errMessage};

            if (Util.OasisBranches.Contains(hospitalId))
            {
                var tempVar = apiCaller.GetOnlineConsultationAmount(lang, hospitalId, patientId, doctorId,
                    doctorScheduleId, billType, ref errStatus, ref errMessage);
                
                if (errStatus == 1 && tempVar != null)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = tempVar;
                }
            }
            else
            {
                var tempVar = _physicianDb.GetConsultationAmountdDataTable(lang, hospitalId, patientId, doctorId,
                    doctorScheduleId, billType, ref errStatus, ref errMessage);

                if (errStatus == 1 && tempVar != null)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = tempVar;
                }

            }
            return Ok(_resp);
        }



        [HttpPost]
        [Route("doctor-days-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsScheduledDays(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["physician_id"]) && col["physician_id"] != "0")
                {
                    var lang = col["lang"];
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    var clinicId = 0;
                    

                    if (!string.IsNullOrEmpty(col["clinic_id"]))
                        clinicId = Convert.ToInt32(col["clinic_id"]);


                    var physicianId = Convert.ToInt32(col["physician_id"]);
                    var selectedDate = System.DateTime.Today;// Convert.ToDateTime(col["date"]);
                    
                    // currently disable the Page Size
                    //var pageNo = Convert.ToInt32(col["page_no"]);
                    //var pageSize = Convert.ToInt32(col["page_size"]);

                    var physicianDb = new PhysicianDB();
                    var allAvailableSlots = physicianDb.GetAvailableDaysOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate);

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
                else
                {
                    _resp.msg = "Failed! Missing Parameter";
                }
            }
            catch(Exception ex)
            {                
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }

            

            return Ok(_resp);
        }

        
        [HttpPost]
        [Route("doctor-slots-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsSlots(FormDataCollection col)
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

                    if (!Util.OasisBranches.Contains(hospitaId))
                    {
                        allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                    }
                    else
                    {
                        allAvailableSlots = availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate);
                        //if (pageno > -1)
                        //{
                        //    allAvailableSlots = allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                        //}
                    }
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






        //new Versrion for Mobile App

        [HttpPost]
        [Route("v2/doctors-AdvanceSearch")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctors_AdvanceSearch(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            string hospitalId = null, clinicId = null, SpecialityName = null, SubSpecialityName = null,
                AssistArea = null , SpokenLanguage =null , GeneralSearch = null;

            var lang = "EN";
            var pageNo = -1;
            var pageSize = 10;
            var IsVideo = 0;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = col["hospital_id"];

                if (!string.IsNullOrEmpty(col["clinic_id"]))
                    clinicId = col["clinic_id"];

                if (!string.IsNullOrEmpty(col["Speciality_name"]))
                    SpecialityName = col["Speciality_name"];


                if (!string.IsNullOrEmpty(col["SubSpeciality"]))
                    SubSpecialityName = col["SubSpeciality"];

                if (!string.IsNullOrEmpty(col["AssistArea"]))
                    AssistArea = col["AssistArea"];

                if (!string.IsNullOrEmpty(col["SpokenLanguage"]))
                    SpokenLanguage = col["SpokenLanguage"];

                if (!string.IsNullOrEmpty(col["GeneralSearch"]))
                    GeneralSearch = col["GeneralSearch"];


                if (!string.IsNullOrEmpty(col["IsVideo"]))
                    IsVideo = Convert.ToInt32(col["IsVideo"]);
            }



            var ApiSource = "MobileApp";
            if (!string.IsNullOrEmpty(col["Sources"]))
                ApiSource = col["Sources"].ToString();

            var allPhysician = _physicianDb.GetPhsiciansAdvanceSearchDT(lang, hospitalId, clinicId, SpecialityName, SubSpecialityName, AssistArea,SpokenLanguage, GeneralSearch, pageNo, pageSize, ApiSource , IsVideo);

            if (allPhysician != null && allPhysician.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Record(s) Found";
                _resp.response = allPhysician;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Record not Failed";
            }




            return Ok(_resp);

        }



        [HttpPost]
        [Route("v2/doctors-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctors_by_Speciality_replica(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            var hospitalId = "";
            var clinicId = "";
            var SpecialityName = "";
            var lang = "EN";
            var pageNo = -1;
            var pageSize = 10;
            hospitalId = null;
            clinicId = null;
            SpecialityName = null;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = col["hospital_id"];

                if (!string.IsNullOrEmpty(col["clinic_id"]))
                    clinicId = col["clinic_id"];

                if (!string.IsNullOrEmpty(col["Speciality_name"]))
                    SpecialityName = col["Speciality_name"];
            }

            var allPhysician = _physicianDb.GetPhsiciansBySpecialityDataTable(lang, hospitalId, clinicId, SpecialityName, pageNo, pageSize);


            if (allPhysician != null && allPhysician.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Record(s) Found";
                _resp.response = allPhysician;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Record not Failed";
            }




            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/Specialty-doctors-list")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctors_by_Speciality(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            var hospitalId = "";
            var clinicId = "";
            var SpecialityName = "";
            var lang = "EN";
            var pageNo = -1;
            var pageSize = 10;
            hospitalId = null;
            clinicId = null;
            SpecialityName = null;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = col["hospital_id"];

                if (!string.IsNullOrEmpty(col["clinic_id"]))
                    clinicId = col["clinic_id"];

                if (!string.IsNullOrEmpty(col["Speciality_name"]))
                    SpecialityName = col["Speciality_name"];
            }


            var ApiSource = "MobileApp";
            if (!string.IsNullOrEmpty(col["Sources"]))
                ApiSource = col["Sources"].ToString();

            var allPhysician = _physicianDb.GetPhsiciansBySpecialityDataTable(lang, hospitalId, clinicId, SpecialityName, pageNo, pageSize, ApiSource);
                

            if (allPhysician != null && allPhysician.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Record(s) Found";
                _resp.response = allPhysician;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Record not Failed";
            }

            
            

            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/doctorprofile-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctors_Profile(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            var hospitalId = "";
            var clinicId = "";
            var SpecialityName = "";
            var DocotrID = "";
            var lang = "EN";
            var pageNo = -1;
            var pageSize = 10;
            hospitalId = null;
            clinicId = null;
            SpecialityName = null;
            DocotrID = null;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = col["hospital_id"];

                if (!string.IsNullOrEmpty(col["clinic_id"]))
                    clinicId = col["clinic_id"];

                if (!string.IsNullOrEmpty(col["Speciality_name"]))
                    SpecialityName = col["Speciality_name"];

                if (!string.IsNullOrEmpty(col["physician_id"]))
                    DocotrID = col["physician_id"];
            }

            _resp.status = 0;
            _resp.msg = "Record not Found";


            if (DocotrID != null && hospitalId == null)
            {
                _resp.status = 0;
                _resp.msg = "Missing Parameter";
            }
            else
            {

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                var allPhysician = _physicianDb.GetAllDoctorsProfile(lang, hospitalId, clinicId, SpecialityName, DocotrID, pageNo, pageSize , ApiSource);
                if (allPhysician != null)
                {   
                    if (allPhysician.DoctorDataList != null && allPhysician.DoctorDataList.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allPhysician;
                    }
                }                
            }
            return Ok(_resp);
        }




        [HttpPost]
        [Route("v2/doctors-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsbyClinic_Copy_v2(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["clinic_id"]) && col["clinic_id"] != "0")
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
                var clinicId = Convert.ToInt32(col["clinic_id"]);


                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize);


                if (allPhysician != null && allPhysician.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allPhysician;

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
        [Route("v2/Mydoctors-get")]
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

                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, clinicId, pageNo, pageSize , true, registrationNo);


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
        [Route("v2/doctors-all")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsbyHospital_v2(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _physicianDb = new PhysicianDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]))
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



                var allPhysician = _physicianDb.GetAllPhsiciansDataTable(lang, hospitalId, 0, pageNo, pageSize);


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
        [Route("v2/doctor-fee")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostConsultationAmount_v2(FormDataCollection col)

        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientId = Convert.ToInt32(col["patient_id"]);
            var doctorId = Convert.ToInt32(col["doctor_id"]);
            var doctorScheduleId = Convert.ToInt32(col["doctor_schedule_id"]);
            var billType = col["bill_type"];
            //ConsultationAmount consultationAmount = null;


            OnlinePaymentApiCaller apiCaller = new OnlinePaymentApiCaller();

            _physicianDb = new PhysicianDB();


            int errStatus = 0;
            string errMessage = "";




            _resp = new GenericResponse { status = 0, msg = errMessage };

            if (Util.OasisBranches.Contains(hospitalId))
            {
                var tempVar = apiCaller.GetOnlineConsultationAmount(lang, hospitalId, patientId, doctorId,
                    doctorScheduleId, billType, ref errStatus, ref errMessage);

                if (errStatus == 1 && tempVar != null)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = tempVar;
                }
            }
            else
            {
                var tempVar = _physicianDb.GetConsultationAmountdDataTable(lang, hospitalId, patientId, doctorId,
                    doctorScheduleId, billType, ref errStatus, ref errMessage);

                if (errStatus == 1 && tempVar != null)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = tempVar;
                }

            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/doctor-days-get")]
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
                    // Convert.ToDateTime(col["date"]);

                    // currently disable the Page Size
                    //var pageNo = Convert.ToInt32(col["page_no"]);
                    //var pageSize = Convert.ToInt32(col["page_size"]);

                    var physicianDb = new PhysicianDB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    //for Video Call Selection 
                    var IsVideo = 0;                    
                    if (!string.IsNullOrEmpty(col["IsVideo"]))
                        IsVideo = Convert.ToInt32(col["IsVideo"]);

                    var allAvailableSlots = physicianDb.GetAvailableDaysOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate, ApiSource , IsVideo);

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
        [Route("v2/doctor-Onlyday-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsScheduledOnlyDays_v2(FormDataCollection col)
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


                    var tempStrDate = col["startdate"];

                    var selectedDate = Convert.ToDateTime(System.DateTime.Today.ToString());
                    if (!string.IsNullOrEmpty(col["startdate"]))
                        selectedDate = Convert.ToDateTime(col["startdate"]);
                    // Convert.ToDateTime(col["date"]);

                    // currently disable the Page Size
                    //var pageNo = Convert.ToInt32(col["page_no"]);
                    //var pageSize = Convert.ToInt32(col["page_size"]);

                    var physicianDb = new PhysicianDB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    //var allAvailableSlots = physicianDb.GetAvailableOnlyDayOfDoctors(lang, hospitalId, clinicId, physicianId, selectedDate, ApiSource);
                    var allAvailableSlots = physicianDb.GetAvailableOnlyDayOfDoctors_TEMPTEST(lang, hospitalId, clinicId, physicianId, tempStrDate, ApiSource);

                    if (allAvailableSlots == null || allAvailableSlots.Rows.Count <= 0)
                    {
                        _resp.status = 0;
                        _resp.msg = "Doctor Schedules are Not available for Selected Doctor on selected Day";
                    }
                    else
                    {
                        _resp.status = 1;
                        _resp.msg = "Doctor Schedules are available for selected Doctor";
                        _resp.response = allAvailableSlots;
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
        [Route("v2/doctor-slots-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorsSlots_v2(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"])  && !string.IsNullOrEmpty(col["physician_id"]) &&
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


                    allAvailableSlots = physicianDb.GetAvailableSlotsByPhysician(lang, hospitaId, clinicId, physicianId, selectedDate, ApiSource , IsVideo);

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




    }
#pragma warning restore 1591
}