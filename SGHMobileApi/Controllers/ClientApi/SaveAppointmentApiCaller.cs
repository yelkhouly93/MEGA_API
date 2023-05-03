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
    public class SaveAppointmentApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        CommonDB _commonDb = new CommonDB();

        string apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
        string apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
        string apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

        public void SaveAppointment(string lang, int hospitalID, int clinicId, int physicianId, DateTime selectedDate, int patientID, DateTime timeFrom, DateTime timeTo, int scheduleDayId, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string SaveAppointmentUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_SaveAppointment_" + hospitalID.ToString()].ToString();

            int patientIDFromDatabase = _commonDb.GetPateintIdAgainstMrn(hospitalID, patientID);

            SaveAppointmentRequestBody requestBody = new SaveAppointmentRequestBody() { appointmentId = scheduleDayId.ToString() };

            SaveAppointmentUrl = SaveAppointmentUrl.Replace("{patientId}", patientIDFromDatabase.ToString());


            var response = RestUtility.CallService<object>(SaveAppointmentUrl, null, requestBody, "POST", apiUserName, apiPassword, out status) as object;
            if (status == HttpStatusCode.OK)
            {
                Er_Status = 1;
                Msg = "Successfully Saved.";
            }
            else
            {
                Er_Status = 0;
                Msg = RestUtility.Msg;
            }

        }

    
    }
}