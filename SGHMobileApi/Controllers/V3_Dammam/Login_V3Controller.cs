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
using SGHMobileApi.Common;
using SmartBookingService.Controllers.ClientApi;
using System.Data;
using System.Configuration;
using DataLayer.Common;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class Login_V3Controller : ApiController
    {
        private readonly EncryptDecrypt_New util = new EncryptDecrypt_New();

        [HttpPost]
        [Route("v3/login-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_V3(FormDataCollection col)
        {
            var resp = new GenericResponse();

            var lang = "EN";
            var hospitalId = 0;
            var patientMrn = 0;

            var IsEncrypt = true;

            if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
            {
                var Source = "";

                if (!string.IsNullOrEmpty(col["Source"]))
                    Source = col["Source"];

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["IsEncrypt"]))
                {
                    if (col["IsEncrypt"] == "0")
                        IsEncrypt = false;
                }






                try
                {
                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);

                    patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                }
                catch (Exception ex)
                {
                    resp.status = 0;
                    resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                    return Ok(resp);
                }


                var PatientNationId = col["patient_national_id"];
                var PCell = col["patient_phone"];

                var loginDb = new Login2DB();
                var errStatus = 0;
                var errMessage = "";
                var OTP = "";

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    if (string.IsNullOrEmpty(col["hospital_id"]))
                    {

                        resp.status = 0;
                        resp.msg = "Missing Prameter. With MRN Please Provide Hospital ID";
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);

                    }
                }

                if (!string.IsNullOrEmpty(col["patient_national_id"]))
                {
                    if (PatientNationId.Length != 10)
                    {
                        resp.status = 0;
                        resp.msg = "Wrong input! Invalid National ID";
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);
                    }
                }


                // For Damamam Intregaration 
                bool CheckInDammam = true;
                bool OnlyDammam = false;

                //checked IF PATIENT SELECT ANY OTHER BRANCH
                if (hospitalId > 0 && hospitalId != 9)
                    CheckInDammam = false;

                // Check Damam MRN Provided
                if (patientMrn > 0 && hospitalId == 9)
                    OnlyDammam = true;





                // For Damamam Intregaration 

                // Ahsan New Chjange for Dammam

                var userInfo = new List<login_check_modal>();

                //First Check in Other Branches - Old Logic
                if (!OnlyDammam)
                    userInfo = loginDb.login_check(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, IsEncrypt);


                // Call dammam API Function fill list
                LoginApiCaller _loginApiCaller = new LoginApiCaller();
                List<login_check_modal> _damuserInfo = new List<login_check_modal>();

                if (CheckInDammam)
                {
                    var IdType = "";
                    var IdValue = "";
                    if (OnlyDammam)
					{
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                    }
					else
					{
                        if (!string.IsNullOrEmpty (PatientNationId))
						{
                            IdType = "NID";
                            IdValue = PatientNationId.ToString();
                            //IdValue = "1174129823";
                        }
                        else if (!string.IsNullOrEmpty(PCell))
						{
                            IdType = "MOB";

                            var TempPcell = PCell.ToString();

                            // change the the Format to
                            TempPcell = TempPcell.Replace("%2B966", "");
                            TempPcell = TempPcell.Replace("+", "");

                            //TempPcell = TempPcell.Substring(0,3).Replace("966", "");                            
                            if (TempPcell.Substring(0, 5) == "00966")
                            {
                                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                            }
                            if (TempPcell.Substring(0, 3) == "966")
							{
                                TempPcell = TempPcell.Substring(3, TempPcell.Length-3);
                            }
                            var FirstChar = TempPcell.Substring(0,1);
                            if (FirstChar != "0")
							{
                                TempPcell = "0" + TempPcell;
                            }


                            IdValue = TempPcell.ToString();
                        }

                    }


                    // Call dammam API Function fill list
                    //LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    //List<login_check_modal> _damuserInfo;
                    _damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang,IdValue, IdType,  ref errStatus, ref errMessage);
                }

                int Tempcount1 = userInfo.Count;
                
                userInfo.AddRange(_damuserInfo);

                int Tempcount2 = userInfo.Count;

                errStatus = 1; // SET initial Error No record Found
                // Now check Count 
                if (userInfo.Count > 0)
				{
                    if (userInfo.Count == 1)
                        errStatus = 0; // SET to zero only one record to send SMS Directly one that Number
                    else
                        errStatus = 2; // Multiple Record Found

                }

                if (errStatus != 1)
                {
                    resp.status = errStatus;
                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        //string smsRes = "";
                        //DataRow dr = userInfo.Rows[0];
                        //var PhoneNumber = dr["PatientCellNo2"].ToString();
                        //userInfo.Rows[0]["PatientCellNo2"] = "";


                        // SENT OTP 
                        int activationCode = 0, ErrorCode;

                        //_loginApiCaller.GenerateOTP_V3(util.Decrypt (userInfo[0].BranchId , true) , userInfo[0].PatientCellNo2, util.Decrypt( userInfo[0].Registrationno , true) , userInfo[0].PatientId , Source ,ref activationCode ,ref errStatus , ref errMessage);
                        _loginApiCaller.GenerateOTP_V3(userInfo[0].BranchId,  userInfo[0].PatientCellNo2, userInfo[0].Registrationno, userInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


                        // Encrpt the Data here For condition if 1 record Found


                        var PhoneNumber = userInfo[0].PatientCellNo2;
                        userInfo[0].PatientCellNo2 = "";

                        //PhoneNumber = "0592285955";
                        OTP = activationCode.ToString();
                        string MsgContent = "";
                        //OTP = "1111";
                        if (OTP != "6465" && OTP != "1122")
                        {
                            //PhoneNumber = "0581178188";
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                            Util.SendTestSMS(PhoneNumber, MsgContent);                            
                        }

                    }
                    else
					{
                        // Loop to Empty Mobile Number 
					}

                    // Encrpt the Data here For condition before sending to API
                    
                    var Final_userInfo = new List<login_check_modal>();
                    //Final_userInfo = userInfo;
                    if (IsEncrypt)
                       Final_userInfo = loginDb.Encrpt_UserList_Obj(userInfo);
                    else
                        Final_userInfo = userInfo;



                    resp.msg = errMessage;
                    resp.response = Final_userInfo;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = errMessage;
                    resp.error_type = errStatus.ToString();
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }


            return Ok(resp);
        }

        [HttpPost]
        [Route("v3/verify-otp")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerificationCode_V2(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                // Ahsan testing Change only Given MRN on Verification
                var loginDb = new Login2DB();

                var lang = "EN";
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = col["patient_reg_no"];
                var patientPhone = col["patient_phone"];
                var patientNationalId = col["patient_national_id"];
                var verificationCode = col["verification_code"];


                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var errStatus = 0;
                var errMessage = "";

                //UserInfo userInfo = loginDb.ValidateLoginUser(lang, hospitalId, null, patientMrn, patientNationalId, ref activationNo, ref errStatus, ref errMessage);
                var userInfo  = new UserInfo_New();
                if (hospitalId==9)
				{
                    var apiCaller = new PatientApiCaller();
                    var IdType = "";
                    var IdValue = "";
                    IdType = "MRN";
                    IdValue = patientMrn.ToString();

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                    // TESTING
                    //userInfo.phone = "0581178188";

                }
                else
				{
                    userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, patientMrn, patientNationalId, ref errStatus, ref errMessage);
                }
                


                var patientDb = new PatientDB();
                //var dt = patientDb.GetPatientDataDT(lang, hospitalId, Convert.ToInt32(patientMrn), ref errStatus, ref errMessage);

                if (string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    // Get MRN using 
                    patientMrn = userInfo.registration_no;

                }
                if (string.IsNullOrEmpty(col["patient_phone"]))
                {
                    // Get PHONE using 
                    patientPhone = userInfo.phone;

                }


                var verifiedCode = patientDb.VerifyOTP(hospitalId, patientMrn, patientPhone, verificationCode);

                if (verifiedCode != null && verifiedCode == verificationCode)
                {

                    //var dt = patientDb.GetPatientDataDT_V2(lang, hospitalId, Convert.ToInt32 (patientMrn), ref errStatus, ref errMessage);
                    resp.response = userInfo;
                    resp.msg = "OTP Verified";
                    resp.status = 1;
                }
                else
                {
                    resp.error_type = "invalid_or_mismatched_otp";
                    resp.msg = "OTP Not Verified";
                    resp.status = 0;
                }
            }
            else if (!string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_phone"]))
            {
                //new Change Logic For Mobile Only                 
                var patientDb = new PatientDB();
                var lang = "EN";
                var hospitalId = 0;   //Optional

                var patientPhone = col["patient_phone"];
                var verificationCode = col["verification_code"];
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = Convert.ToInt32(col["hospital_id"]);


                var errStatus = 0;
                var errMessage = "";

                var verifiedCode = patientDb.VerifyOTP_MobileOnly(patientPhone, verificationCode);

                if (verifiedCode != null && verifiedCode == verificationCode)
                {
                    resp.msg = "OTP Verified";
                    resp.status = 1;

                    var userInfo = patientDb.GetPatientList_ByMobile(lang, patientPhone, hospitalId, ref errStatus, ref errMessage);
                    resp.response = userInfo;
                }
                else
                {
                    resp.error_type = "invalid_or_mismatched_otp";
                    resp.msg = "OTP Not Verified";
                    resp.status = 0;
                }
            }
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified";
                resp.status = 0;
            }

            return Ok(resp);
        }

    }
}
