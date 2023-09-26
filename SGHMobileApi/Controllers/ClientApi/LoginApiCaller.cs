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
    public class LoginApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public UserInfo ValidateLoginUserByApi(string lang, int hospitalID, string PCellNo, string RegistrationNo, string NationalId, ref int activationNo, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            UserInfo _userInfo = new UserInfo();
            accountData _accData = new accountData();
            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string RegistrationUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_RetreivePatientInfo_" + hospitalID.ToString()].ToString();

            var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
            var apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

            if (RegistrationNo != "" && RegistrationNo != null)
            {
                _accData = new accountData() { idNumber = RegistrationNo, idType = "MRN" };
            }
            else
            {
                _accData = new accountData() { idNumber = NationalId, idType = "ID" };
            }

            patientData _data = RestUtility.CallService<patientData>(RegistrationUrl, null, _accData, "POST", apiUserName, apiPassword, out status) as patientData;

            var _userInfoModel = new List<UserInfo2Model>();

            DB.param = new SqlParameter[]
                {
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PCellNo", _data != null ? _data.MOBILE_NO : ""),
                new SqlParameter("@RegistrationNo", _data != null ? _data.MRN.ToString() : "0"),
                new SqlParameter("@NationalId", NationalId),
                new SqlParameter("@PatientId", _data != null ? _data.PATIENT_ID : 0),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", status == HttpStatusCode.OK ? 0 : 1),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
                };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.InputOutput;
            DB.param[7].Direction = ParameterDirection.Output;

            _userInfoModel = DB.ExecuteSPAndReturnDataTable("dbo.Generate_OTP").ToListObject<UserInfo2Model>();

            Er_Status = Convert.ToInt32(DB.param[6].Value);
            Msg = DB.param[7].Value.ToString();

            if (Er_Status == 0)
                activationNo = Convert.ToInt32(DB.param[5].Value);

            if (status == HttpStatusCode.OK)
            {
                _userInfo = MapUserInfoModelToUserInf(_data);
            }

            return _userInfo;

        }

        private UserInfo MapUserInfoModelToUserInf(patientData _userInfoModel)
        {
            UserInfo _userInfo = new UserInfo();

            _userInfo.address = "";
            _userInfo.birthday = Convert.ToDateTime(_userInfoModel.DATE_OF_BIRTH);
            _userInfo.email = _userInfoModel.EMAIL_ADDRESS;
            _userInfo.family_name = _userInfoModel.PAT_NAME_FAMILY;
            _userInfo.first_name = _userInfoModel.PAT_NAME_1;
            _userInfo.gender = Convert.ToInt32(_userInfoModel.SEX != null ? Util.GetSexID(_userInfoModel.SEX.ToString()) : "0");  //getGenderId(_userInfoModel.SEX)
            _userInfo.hospital_id = 10;
            _userInfo.id = _userInfoModel.PATIENT_ID;
            _userInfo.last_name = _userInfoModel.PAT_NAME_3;
            _userInfo.marital_status_id = 0;
            _userInfo.middle_name = _userInfoModel.PAT_NAME_2;
            _userInfo.name = _userInfoModel.PATIENT_NAME;
            _userInfo.national_id = "";
            _userInfo.phone = _userInfoModel.MOBILE_NO;
            //_userInfo.registration_no = _userInfoModel.MRN;
            _userInfo.registration_no = _userInfoModel.MRN.ToString();
            _userInfo.title_id = 0;
            _userInfo.name_ar = _userInfoModel.PATIENT_NAME;

            return _userInfo;
        }



        public UserInfo ValidateLoginUserByApi_NewDam(string lang, string PCellNo, string RegistrationNo, string NationalId, ref int activationNo, ref int Er_Status, ref string Msg)
        {

            ////HttpStatusCode status;
            ////UserInfo _userInfo = new UserInfo();
            //accountData _accData = new accountData();
            ////string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_"]; ;
            ////string RegistrationUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_RetreivePatientInfo_"];
            //string RegistrationUrl = "";
            ////var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            ////var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
            ////var apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

            ////         if (RegistrationNo != "" && RegistrationNo != null)
            ////         {
            ////             _accData = new accountData() { idNumber = RegistrationNo, idType = "MRN" };
            ////         }
            ////         else if (PCellNo != "" && PCellNo!= null)
            ////{
            ////             _accData = new accountData() { idNumber = RegistrationNo, idType = "MRN" };
            ////         }
            ////         else
            ////         {
            ////             _accData = new accountData() { idNumber = NationalId, idType = "ID" };
            ////         }

            //_accData = new accountData() { idNumber = RegistrationNo, idType = "MRN" };

            //var _data = RestUtility.CallService<List<patientData_Dam>>(RegistrationUrl, null, _accData, "POST", null, null, out status) as List<patientData_Dam>;

            //var _userInfoModel = new List<UserInfo2Model>();

            //DB.param = new SqlParameter[]
            //    {
            //    //new SqlParameter("@BranchId", hospitalID),
            //    //new SqlParameter("@PCellNo", _data != null ? _data.MOBILE_NO : ""),
            //    //new SqlParameter("@RegistrationNo", _data != null ? _data.MRN.ToString() : "0"),
            //    new SqlParameter("@NationalId", NationalId),
            //    //new SqlParameter("@PatientId", _data != null ? _data.PATIENT_ID : 0),
            //    new SqlParameter("@ACtivationNo", SqlDbType.Int),
            //    //new SqlParameter("@Er_Status", status == HttpStatusCode.OK ? 0 : 1),
            //    new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
            //    };
            //DB.param[5].Direction = ParameterDirection.Output;
            //DB.param[6].Direction = ParameterDirection.InputOutput;
            //DB.param[7].Direction = ParameterDirection.Output;

            //_userInfoModel = DB.ExecuteSPAndReturnDataTable("dbo.Generate_OTP").ToListObject<UserInfo2Model>();

            //Er_Status = Convert.ToInt32(DB.param[6].Value);
            //Msg = DB.param[7].Value.ToString();

            //if (Er_Status == 0)
            //    activationNo = Convert.ToInt32(DB.param[5].Value);

            ////if (status == HttpStatusCode.OK)
            ////{
            ////    _userInfo = MapUserInfoModelToUserInf(_data);
            ////}

            ////return _userInfo;
            return null;

        }


    }
}