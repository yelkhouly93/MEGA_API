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
    public class NewPatientRegistrationApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public List<RegistePatientResponseFailure> RegisterNewPatient(RegisterPatient2 registerPatient, ref int Er_Status, ref string msg, ref string error_type, ref string successType, ref int RegistrationNo, ref int activationNo)
        {
            try
            {
                List<RegistePatientResponseFailure> registerPatientFailure = new List<RegistePatientResponseFailure>();
                HttpStatusCode status;
                UserInfo _userInfo = new UserInfo();
                accountData _accData = new accountData();

                string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + registerPatient.HospitaId.ToString()].ToString();

                string NewUserRegistrationUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_NewPateintRegister_" + registerPatient.HospitaId.ToString()].ToString();

                var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
                var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
                var apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

                NewUserRegsitrationFromApi requestBody = new NewUserRegsitrationFromApi();
                requestBody.dateOfBirth = registerPatient.PatientBirthday;
                requestBody.familyName = registerPatient.PatientLastName;
                requestBody.firstName = registerPatient.PatientFirstName;
                requestBody.idNumber = registerPatient.PatientNationalId;
                requestBody.secandName = registerPatient.PatientMiddleName;
                requestBody.nationalityCode = registerPatient.PatientNationalityId;
                requestBody.sex = registerPatient.PatientGender == 1 ? "F" : "M";
                requestBody.mobileNo = registerPatient.PatientPhone.Replace("+966", "0");

                var response = RestUtility.CallService<object>(NewUserRegistrationUrl, null, requestBody, "POST", apiUserName, apiPassword, out status) as object;
                
                DB.param = new SqlParameter[]
                    {
                new SqlParameter("@BranchId", registerPatient.HospitaId),
                new SqlParameter("@PCellNo", registerPatient != null ? registerPatient.PatientPhone : ""),
                new SqlParameter("@RegistrationNo", response != null ? Convert.ToInt32(response) : 0),
                new SqlParameter("@NationalId", registerPatient.PatientNationalId),
                new SqlParameter("@PatientId", response != null ? Convert.ToInt32(response) : 0),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", status == HttpStatusCode.OK ? 0 : 1),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
                    };
                DB.param[5].Direction = ParameterDirection.Output;
                DB.param[6].Direction = ParameterDirection.InputOutput;
                DB.param[7].Direction = ParameterDirection.Output;

                var _userInfoModel = DB.ExecuteSPAndReturnDataTable("dbo.Generate_OTP").ToListObject<UserInfo2Model>();

                //Er_Status = Convert.ToInt32(DB.param[6].Value);
                //Msg = DB.param[7].Value.ToString();

                //if (Er_Status == 0)
                //    activationNo = Convert.ToInt32(DB.param[5].Value);

                if (status == HttpStatusCode.OK)
                {
                    Er_Status = 1;
                }
                else
                {
                    Er_Status = 0;
                }

                return registerPatientFailure;

    }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return null;
            }
            

        }
    }
}