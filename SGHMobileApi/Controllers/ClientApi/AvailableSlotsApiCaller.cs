using DataLayer.Common;
using DataLayer.Data;
using DataLayer.Model;
using Newtonsoft.Json.Linq;
using RestClient;
using SGHMobileApi.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace SmartBookingService.Controllers.ClientApi
{
    public class AvailableSlotsApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        CommonDB _commonDb = new CommonDB();

        string apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
        string apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
        string apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

        public List<AvailableDays> GetAllClinicsByApi(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate)
        {
            HttpStatusCode status;
            var _allAvailableDaysModel = new List<AvailableDaysModelApi>();
            List<AvailableDays> _allAvailableDays = new List<AvailableDays>();

            clinicID = _commonDb.GetClinicIdAgainstPhysician(hospitalID, physicianID);

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string GetSpecialityStaffListUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_GetSpecialityStaffList_" + hospitalID.ToString()].ToString();
            

            GetSpecialityStaffListUrl = GetSpecialityStaffListUrl.Replace("{patientId}", apiFixedPatientId);
            GetSpecialityStaffListUrl = GetSpecialityStaffListUrl.Replace("{speciality}", clinicID.ToString());

            _allAvailableDaysModel = RestUtility.CallService<List<AvailableDaysModelApi>>(GetSpecialityStaffListUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<AvailableDaysModelApi>;

            _allAvailableDays = MapAvailableDaysModelToAvailableDays(_allAvailableDaysModel, physicianID, hospitalID);

            return _allAvailableDays;

        }

        private List<AvailableDays> MapAvailableDaysModelToAvailableDays(List<AvailableDaysModelApi> _allAvailableSlotsModel, int physicianID, int hospitalID)
        {
            List<AvailableDays> _allAvailableDays = new List<AvailableDays>();
            var doctorCode = _commonDb.GetDcotorCodeAgainstPhysician(hospitalID, physicianID);
            dynamic _availableDays;


            if (_allAvailableSlotsModel != null)
            {
                if (hospitalID == 5)
                {
                    _availableDays = _allAvailableSlotsModel.Where(x => x.STAFF_LIST.Any(y => y.CONSULTANT == doctorCode.ToString()));
                }
                else {
                    _availableDays = _allAvailableSlotsModel.Where(x => x.STAFF_LIST.Any(y => y.CONSULTANT == physicianID.ToString()));
                }
                

                int seq = 1;

                foreach (var availableDay in _availableDays)
                {
                    if (Convert.ToDateTime(availableDay.DATE) > DateTime.Now.Date)
                    {
                        AvailableDays _availableSlot = new AvailableDays();
                        _availableSlot.Id = seq++;
                        _availableSlot.doctor_id = physicianID;
                        _availableSlot.schedule_day = Convert.ToDateTime(availableDay.DATE);//.ToShortDateString();

                        _allAvailableDays.Add(_availableSlot);
                    }

                }
            }


            return _allAvailableDays;
        }

        public List<AvailableSlots> GetAvailableSlotsByPhysician(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate, int pageno = -1, int pagesize = 10)
        {

            HttpStatusCode status;
            var _allAvailableSlotsModel = new List<AvailableSlotsModelApi>();
            List<AvailableSlots> _allAvailableSlots = new List<AvailableSlots>();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string GetAvaialbleSlotsUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_GetStaffAvaialableSlots_" + hospitalID.ToString()].ToString();

            GetAvaialbleSlotsUrl = GetAvaialbleSlotsUrl.Replace("{patientId}", apiFixedPatientId);

            if (hospitalID == 5)
            {
                var employeeCode = _commonDb.GetDcotorCodeAgainstPhysician(hospitalID, physicianID);
                GetAvaialbleSlotsUrl = GetAvaialbleSlotsUrl.Replace("{consultant}", employeeCode.ToString());
            }
            else {
                GetAvaialbleSlotsUrl = GetAvaialbleSlotsUrl.Replace("{consultant}", physicianID.ToString());
            }
            


            GetAvaialbleSlotsUrl += "?date=" + selectedDate.ToString("yyyy-MM-dd");

            _allAvailableSlotsModel = RestUtility.CallService<List<AvailableSlotsModelApi>>(GetAvaialbleSlotsUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<AvailableSlotsModelApi>;

            _allAvailableSlots = MapAvailableSlotsModelToAvailableSlots(_allAvailableSlotsModel);

            return _allAvailableSlots;

        }

        private List<AvailableSlots> MapAvailableSlotsModelToAvailableSlots(List<AvailableSlotsModelApi> _allAvailableSlotsModel)
        {
            List<AvailableSlots> _allAvailableSlots = new List<AvailableSlots>();

            foreach (var availableSlotsModel in _allAvailableSlotsModel)
            {
                AvailableSlots _availableSlot = new AvailableSlots();
                _availableSlot.Id = availableSlotsModel.appointmentId;
                _availableSlot.time_from = availableSlotsModel.startDate.ToString("HH:mm:ss");
                _availableSlot.time_to = availableSlotsModel.startDate.AddMinutes(10).ToString("HH:mm:ss");
                _availableSlot.slot_type_id = 1;
                _availableSlot.slot_type_name = "In Clinic";

                _allAvailableSlots.Add(_availableSlot);

            }

            return _allAvailableSlots;

        }
    }
}