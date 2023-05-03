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
using SGHMobileApi.Common;
using System.Linq;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class AvailableSlotsController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Get available slots against physician and appointment date.
        /// </summary>
        /// <returns>Return availabke slots against physician and appointment date</returns>
        [HttpPost]
        [Route("get-slots-of-physician")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {

            
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var clinicID = Convert.ToInt32(col["clinic_id"]);
            var physicianID = Convert.ToInt32(col["physician_id"]);
            var selectedDate = Convert.ToDateTime(col["date"]);
            List<AvailableSlots> _allAvailableSlots;
            AvailableSlotsApiCaller _availableSlotsApiCaller = new AvailableSlotsApiCaller();


            if (!Util.OasisBranches.Contains(hospitaId))
            {
                PhysicianDB _physicianDB = new PhysicianDB();
                _allAvailableSlots = _physicianDB.GetAvailableSlotsByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate);
            }
            else
            {
                _allAvailableSlots = _availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate);
            }

            


            GenericResponse resp = new GenericResponse();

            if (_allAvailableSlots != null && _allAvailableSlots.Count > 0)
            {
                resp.status = 1;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are available for selected Date";
                else
                    resp.msg = "Doctor Schedules are available for selected Date";
                resp.response = _allAvailableSlots;
                
            }
            else
            {
                resp.status = 0;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are Not available for selected Date";
                else
                    resp.msg = "Doctor Schedules are Not available for selected Date";
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("get-slots-of-physician-paged")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post2(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var clinicID = Convert.ToInt32(col["clinic_id"]);
            var physicianID = Convert.ToInt32(col["physician_id"]);
            var selectedDate = Convert.ToDateTime(col["date"]);
            var pageno = Convert.ToInt32(col["page_no"]);
            var pagesize = Convert.ToInt32(col["page_size"]);
            PhysicianDB _physicianDB = new PhysicianDB();
            AvailableSlotsApiCaller _availableSlotsApiCaller = new AvailableSlotsApiCaller();
            List<AvailableSlots> _allAvailableSlots;

            if (!Util.OasisBranches.Contains(hospitaId))
            {
                _allAvailableSlots = _physicianDB.GetAvailableSlotsByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate, pageno, pagesize);
            }
            else
            {
                _allAvailableSlots = _availableSlotsApiCaller.GetAvailableSlotsByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate);
                if (pageno > -1)
                {
                    _allAvailableSlots = _allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                }
            }
            
            


            GenericResponse resp = new GenericResponse();

            if (_allAvailableSlots != null && _allAvailableSlots.Count > 0)
            {
                resp.status = 1;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are available for selected Date";
                else
                    resp.msg = "Doctor Schedules are available for selected Date";
                resp.response = _allAvailableSlots;

            }
            else
            {
                resp.status = 0;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are Not available for selected Date";
                else
                    resp.msg = "Doctor Schedules are Not available for selected Date";
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("api/get-days-of-physician")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetScheduledDays(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var clinicID = Convert.ToInt32(col["clinic_id"]);
                var physicianID = Convert.ToInt32(col["physician_id"]);
                var selectedDate = System.DateTime.Today;// Convert.ToDateTime(col["date"]);
                List<AvailableDays> _allAvailableSlots;
                AvailableSlotsApiCaller _availableSlotsApiCaller = new AvailableSlotsApiCaller();

                if (!Util.OasisBranches.Contains(hospitaId))
                {
                    log.Info("APi Called for Available Days: Hospital ID :  " + hospitaId);
                    log.Info("APi Called for Available Days: Department ID :  " + clinicID);
                    log.Info("APi Called for Available Days: Doctor Id :  " + physicianID);
                    
                    PhysicianDB _physicianDB = new PhysicianDB();
                    _allAvailableSlots = _physicianDB.GetAvailableDaysByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate);
                }
                else
                {
                    log.Info("BranchId : " + hospitaId + " ---- DoctorId : " + physicianID + " ---- DepartmentId : " + clinicID);
                    _allAvailableSlots = _availableSlotsApiCaller.GetAllClinicsByApi(lang, hospitaId, clinicID, physicianID, selectedDate);
                }

                log.Info("Avaialble Slot: " + _allAvailableSlots.Count);
                

                if (_allAvailableSlots != null && _allAvailableSlots.Count > 0)
                {
                    resp.status = 1;
                    if (Convert.ToString(lang) == "en")
                        resp.msg = "Doctor Schedules are available for selected Doctor";
                    else
                        resp.msg = "Doctor Schedules are available for selected Doctor";
                    resp.response = _allAvailableSlots;
                }
                else
                {
                    resp.status = 0;
                    if (Convert.ToString(lang) == "en")
                        resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                    else
                        resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                }

                
            }
            catch (Exception ex)
            {

                log.Error("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Ok(resp);
        }

        [HttpPost]
        [Route("api/get-days-of-physician-paged")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetScheduledDays2(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var clinicID = Convert.ToInt32(col["clinic_id"]);
            var physicianID = Convert.ToInt32(col["physician_id"]);
            var selectedDate = System.DateTime.Today;// Convert.ToDateTime(col["date"]);
            var pageno = Convert.ToInt32(col["page_no"]);
            var pagesize = Convert.ToInt32(col["page_size"]);

            List<AvailableDays> _allAvailableSlots;
            AvailableSlotsApiCaller _availableSlotsApiCaller = new AvailableSlotsApiCaller();

            if (!Util.OasisBranches.Contains(hospitaId))
            {
                PhysicianDB _physicianDB = new PhysicianDB();
                _allAvailableSlots = _physicianDB.GetAvailableDaysByPhysician(lang, hospitaId, clinicID, physicianID, selectedDate, pageno, pagesize);
            }
            else
            {
                _allAvailableSlots = _availableSlotsApiCaller.GetAllClinicsByApi(lang, hospitaId, clinicID, physicianID, selectedDate);
                if (pageno > -1)
                {
                    _allAvailableSlots = _allAvailableSlots.OrderBy(i => i.Id).Skip((pageno) * pagesize).Take(pagesize).ToList();
                }
            }

            GenericResponse resp = new GenericResponse();

            if (_allAvailableSlots != null && _allAvailableSlots.Count > 0)
            {
                resp.status = 1;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are available for selected Doctor";
                else
                    resp.msg = "Doctor Schedules are available for selected Doctor";
                resp.response = _allAvailableSlots;
            }
            else
            {
                resp.status = 0;
                if (Convert.ToString(lang) == "en")
                    resp.msg = "Doctor Schedules are Not available for Selected Doctor";
                else
                    resp.msg = "Doctor Schedules are Not available for Selected Doctor";
            }

            return Ok(resp);
        }
      
    }
}