using DataLayer.Common;
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
    public class OnlinePaymentApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public ConsultationAmount GetOnlineConsultationAmount(string lang, int hospitaId, int patientId, int doctorId, int doctorScheduleId, string billType, ref int errStatus, ref string errMessage)
        {

            HttpStatusCode status;
            var _consultationAmount = new ConsultationAmount();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitaId.ToString()].ToString();
            string OnlineConsultationAmpuntUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_GetOnlineConsultionAmount_" + hospitaId.ToString()].ToString();
            OnlineConsultationAmpuntUrl = OnlineConsultationAmpuntUrl + "?app_id=" + doctorScheduleId + "&pay_type=" + billType + "&lang=en";

            var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();

            _consultationAmount = RestUtility.CallService<ConsultationAmount>(OnlineConsultationAmpuntUrl, null, null, "GET", apiUserName, apiPassword, out status) as ConsultationAmount;

            return _consultationAmount;

        }

        public ConsultationAmount SaveOnlinePaymentAgainstBill(string lang, int hospitaId, int registrationNo, decimal amount, string creditCardType, string ccNumber, DateTime? ccValidity, string onlineTransactionId, int hisRefId, int hisRefTypeId, ref int errStatus, ref string errMessage)
        {

            HttpStatusCode status;
            var _consultationAmount = new ConsultationAmount();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitaId.ToString()].ToString();
            string SaveOnlinePaymentUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_SaveOnlinePayment_" + hospitaId.ToString()].ToString();

            var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();

            _consultationAmount = RestUtility.CallService<ConsultationAmount>(SaveOnlinePaymentUrl, null, null, "GET", apiUserName, apiPassword, out status) as ConsultationAmount;

            return _consultationAmount;

        }

        
    }
}