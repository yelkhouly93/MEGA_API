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
    public class ClinicApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public List<ClinicsByApi> GetAllClinicsByApi(string lang, int hospitalID, int pageno = -1, int pagesize = 10)
        {

            HttpStatusCode status;
            var _allClinics = new List<ClinicsByApi>();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string GetAllSpecilitiesUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_RetreiveAllSpecilities_" + hospitalID.ToString()].ToString();
            
            var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
            var apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();


            _allClinics = RestUtility.CallService<List<ClinicsByApi>>(GetAllSpecilitiesUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<ClinicsByApi>;
            

            return _allClinics;

        }
    }
}