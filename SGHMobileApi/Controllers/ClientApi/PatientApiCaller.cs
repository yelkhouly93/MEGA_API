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
    public class PatientApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        CommonDB _commonDb = new CommonDB();

        string apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
        string apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
        string apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

        public PatientData CheckPatientData(string lang, int hospitalID, int patientID, string pateintPhone, string registrationNo, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;

            

            PatientData _patientData = new PatientData();

            patientData _patientDataFromApi = getPatientData(hospitalID, patientID, out status);

            List<PatientReservations> patientReservationModel = getPatientReservations(hospitalID, patientID, out status);

            _patientData = MapPatinetDataModelToPatientData(_patientDataFromApi, patientReservationModel);

            if (status == HttpStatusCode.OK)
            {
                Er_Status = 1;
                Msg = "Success.";
            }
            else
            {
                Er_Status = 0;
                Msg = RestUtility.Msg;
            }

            return _patientData;
        }

        public patientData getPatientData(int hospitalID, int registrationNo, out HttpStatusCode status)
        {
           
            accountData _accData = new accountData();
            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string RegistrationUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_RetreivePatientInfo_" + hospitalID.ToString()].ToString();


            if (registrationNo.ToString() != ""  && registrationNo != 0)
            {
                _accData = new accountData() { idNumber = registrationNo.ToString(), idType = "MRN" };
            }

            patientData _data = RestUtility.CallService<patientData>(RegistrationUrl, null, _accData, "POST", apiUserName, apiPassword, out status) as patientData;

            return _data;
        }

        public List<PatientReservations> getPatientReservations(int hospitalID, int patientID, out HttpStatusCode status)
        {
           
            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string PatientBookingsUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_FetchPatientBookings_" + hospitalID.ToString()].ToString();

            int patientIDFromDatabase = _commonDb.GetPateintIdAgainstMrn(Convert.ToInt32(hospitalID), patientID);

            PatientBookingsUrl = PatientBookingsUrl.Replace("{patientId}", patientIDFromDatabase.ToString());

            List<PatientReservations> patientReservationsList = RestUtility.CallService<List<PatientReservations>>(PatientBookingsUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<PatientReservations>;

            return patientReservationsList;
        }


        private PatientData MapPatinetDataModelToPatientData(patientData _userInfoModel, List<PatientReservations> patientReservationModel)
        {
            try
            {
                Patient _patient = new Patient();
                List<Reservation> _reservationList = new List<Reservation>();
                PatientData _pateitnData = new PatientData();



                if (_userInfoModel != null)
                {
                    _patient.address = "";
                    _patient.birthday = _userInfoModel.DATE_OF_BIRTH;
                    _patient.email = _userInfoModel.EMAIL_ADDRESS;
                    _patient.family_name = _userInfoModel.PAT_NAME_FAMILY;
                    _patient.first_name = _userInfoModel.PAT_NAME_1;
                    _patient.gender = Convert.ToInt32(_userInfoModel.SEX != null ? Util.GetSexID(_userInfoModel.SEX.ToString()) : "0"); // getGenderId(_userInfoModel.SEX)
                    _patient.hospital_id = 10;
                    _patient.id = _userInfoModel.PATIENT_ID;
                    _patient.last_name = _userInfoModel.PAT_NAME_3;
                    _patient.marital_status_id = 0;
                    _patient.middle_name = _userInfoModel.PAT_NAME_2;
                    _patient.name = _userInfoModel.PATIENT_NAME;
                    _patient.national_id = "";
                    _patient.phone = _userInfoModel.MOBILE_NO;
                    _patient.registration_no = _userInfoModel.MRN.ToString();
                    _patient.title_id = 0;
                    _patient.name = _userInfoModel.PATIENT_NAME;

                }

                _pateitnData.patient = _patient;



                if (patientReservationModel != null && patientReservationModel.Count > 0)
                {
                    foreach (var row in patientReservationModel)
                    {
                        Reservation _reservation = new Reservation();

                        
                        _reservation.clinic_name = row.workEntityDesc;
                        _reservation.date = row.startDate.ToString("yyyy-MM-dd");
                        _reservation.id = row.appointmentId;
                        _reservation.patient_attend = 0;
                        _reservation.patient_name = _userInfoModel.PATIENT_NAME;
                        _reservation.physician_name = row.staffName;
                        _reservation.time_from = row.startDate.ToString("HH:mm:ss");
                        _reservation.paid = 2;
                        _reservation.appointment_type = 1;

                        // Ahsan changes 27/05/2021
                        // Check Logic of Date
                        DateTime combinedDateTime = row.startDate.Date.Add(row.startDate.TimeOfDay);

                        _reservation.UpComingAppointment = combinedDateTime > System.DateTime.Now ? 1 : 0;





                        _reservationList.Add(_reservation);
                    }

                    _pateitnData.reservations = _reservationList;

                }

                return _pateitnData;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

    }
}