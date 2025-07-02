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
using RestClient;

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
				{
                    userInfo = loginDb.login_check(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, IsEncrypt);
                    if (errStatus == 100)
					{
                        resp.status = 0;
                        resp.msg = errMessage;
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);
                    }
                }
                    


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
                    //resp.error_type = "invalid_or_mismatched_otp";
                    //resp.msg = "OTP Not Verified V3 - 1";
                    //resp.status = 0;
                    if (verifiedCode == "EXPIRED")
                    {
                        resp.error_type = "Exceed Limit";
                        resp.msg = "Verification Limit Exceed, Please Login Again!";
                        resp.status = 0;


                    }
                    else
                    {
                        resp.error_type = "invalid_or_mismatched_otp";
                        resp.msg = "OTP Not Verified....";
                        resp.status = 0;
                        
                    }
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
                    resp.msg = "OTP Not Verified..";
                    resp.status = 0;
                }
            }
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified.";
                resp.status = 0;
            }

            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/login-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();

            var lang = "EN";
            var hospitalId = 0;
            var patientMrn = "";

            var IsEncrypt = true;
            if (string.IsNullOrEmpty(col["country_ID"]))
			{
                resp.status = 0;
                resp.msg = "Missing Parameter! Country Required";
                return Ok(resp);
            }

            if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
            {
                var Source = "";
                var CountryId = 0;
                try
				{
                    CountryId = Convert.ToInt32(col["country_ID"]);
                }
                catch (Exception ex)
				{
                    resp.status = 0;
                    resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                    return Ok(resp);
                }
                


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

                    if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                        patientMrn = col["patient_reg_no"].ToString();
                    else
                        patientMrn = "";
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

                if (!string.IsNullOrEmpty(col["patient_national_id"]) && CountryId == 2)
                {
                    if (PatientNationId.Length != 10)
                    {
                        resp.status = 0;
                        resp.msg = "Wrong input! Invalid National ID";
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);
                    }
                }



                if (CountryId == 2) /*for KSA*/
				{
                    // For Damamam Intregaration 
                    bool CheckInDammam = true;
                    bool OnlyDammam = false;
                    var intpatientMrn = 0;


                    if (!string.IsNullOrEmpty(col["patient_reg_no"]))
					{
                        try
                        {
                            intpatientMrn = Convert.ToInt32(patientMrn);
                        }
                        catch (Exception ex)
                        {
                            resp.status = 0;
                            resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                            return Ok(resp);
                        }
                    }

                    


                    //checked IF PATIENT SELECT ANY OTHER BRANCH
                    if (hospitalId > 0 && hospitalId != 9)
                        CheckInDammam = false;

                    // Check Damam MRN Provided
                    if (intpatientMrn > 0 && hospitalId == 9)
                        OnlyDammam = true;

                    // For Damamam Intregaration 
                    // Ahsan New Chjange for Dammam
                    var userInfo = new List<login_check_modal>();

                    //First Check in Other Branches - Old Logic
                    if (!OnlyDammam)
                    {
                        
                        userInfo = loginDb.login_check(lang, hospitalId, PCell, PatientNationId, intpatientMrn, Source, ref errStatus, ref errMessage, IsEncrypt);
                        if (errStatus == 100)
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            resp.error_type = errStatus.ToString();
                            return Ok(resp);
                        }
                    }

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
                            if (!string.IsNullOrEmpty(PatientNationId))
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
                                    TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                                }
                                var FirstChar = TempPcell.Substring(0, 1);
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
                        _damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
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
                            _loginApiCaller.GenerateOTP_V3(userInfo[0].BranchId, userInfo[0].PatientCellNo2, userInfo[0].Registrationno, userInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


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
                else if (CountryId == 3) /*for UAE*/
				{   
                    var IdType = "";
                    var IdValue = "";
                    var BranchName = "";

                    if (!string.IsNullOrEmpty(patientMrn.ToString()))
                    {
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(PatientNationId))
                        {
                            IdType = "IdCard";
                            IdValue = PatientNationId.ToString();                            
                        }
                        else if (!string.IsNullOrEmpty(PCell))
                        {
                            IdType = "Mobile";

                            var TempPcell = PCell.ToString();

                            // change the the Format to
                            TempPcell = TempPcell.Replace("%2B971", "");
                            TempPcell = TempPcell.Replace("%2B", "");
                            TempPcell = TempPcell.Replace("+", "");
                            
                            if (TempPcell.Substring(0, 5) == "00971")
                            {
                                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                            }
                            if (TempPcell.Substring(0, 3) == "971")
                            {
                                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                            }
                            var FirstChar = TempPcell.Substring(0, 1);
                            //if (FirstChar != "0")
                            //{
                            //    TempPcell = "0" + TempPcell;
                            //}


                            IdValue = TempPcell.ToString();
                        }
                    }
                    
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _UAEserInfo = _UAEApiCaller.ValidateLoginUserByApi_NewUAE(lang, IdValue, IdType, hospitalId, ref errStatus, ref errMessage);


                    //if (userInfo.Count > 0)
                    //{
                    //    if (userInfo.Count == 1)
                    //        errStatus = 0; // SET to zero only one record to send SMS Directly one that Number
                    //    else
                    //        errStatus = 2; // Multiple Record Found
                    //}

                    if (_UAEserInfo == null)
					{
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);
                    }
                    
                    if (_UAEserInfo.Count == 0)
					{
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);

                    }
                    else if (_UAEserInfo.Count == 1 )
					{
                        resp.status = 1;
                        // SENT OTP 
                        int activationCode = 0;
                        int ErrorCode = 0;
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        _loginApiCaller.GenerateOTP_V3(_UAEserInfo[0].BranchId, _UAEserInfo[0].PatientCellNo2, _UAEserInfo[0].Registrationno, _UAEserInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


                        // Encrpt the Data here For condition if 1 record Found
                        var PhoneNumber = _UAEserInfo[0].PatientCellNo2;
                        _UAEserInfo[0].PatientCellNo2 = "";
                        OTP = activationCode.ToString();
                        string MsgContent = "";
                        //OTP = "1111";


                        //var CBC = new CommonDB();
                        //CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);






                        //if (OTP != "6465" && OTP != "1122")
                        
                            // UAE SMS API CAll 
                            //PhoneNumber = "0581178188";
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText_UAE"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                            //Util.SendTestSMS(PhoneNumber, MsgContent);
                            var CBC = new CommonDB();
                            CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);
                        
                        errStatus = 1; // Multiple Record Found
                    }
                    else
					{
                        // Multiple Record Found 
                        resp.status = 2;
                        errStatus = 2; // Multiple Record Found

                    }
                    // Encrpt the Data here For condition before sending to API
                    var Final_userInfo = new List<login_check_modal>();
                    //Final_userInfo = userInfo;
                    if (IsEncrypt)
                        Final_userInfo = loginDb.Encrpt_UserList_Obj(_UAEserInfo);
                    else
                        Final_userInfo = _UAEserInfo;

                    resp.msg = errMessage;
                    resp.response = Final_userInfo;
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
        [Route("v6/login-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_V6(FormDataCollection col)
        {
            var resp = new GenericResponse();

            var lang = "EN";
            var hospitalId = 0;
            var patientMrn = "";

            var IsEncrypt = true;
            if (string.IsNullOrEmpty(col["country_ID"]))
            {
                resp.status = 0;
                resp.msg = "Missing Parameter! Country Required";
                return Ok(resp);
            }

            if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
            {
                var Source = "";
                var CountryId = 0;
                try
                {
                    CountryId = Convert.ToInt32(col["country_ID"]);
                }
                catch (Exception ex)
                {
                    resp.status = 0;
                    resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                    return Ok(resp);
                }



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

                    if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                        patientMrn = col["patient_reg_no"].ToString();
                    else
                        patientMrn = "";
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

                if (!string.IsNullOrEmpty(col["patient_national_id"]) && CountryId == 2)
                {
                    if (PatientNationId.Length != 10)
                    {
                        resp.status = 0;
                        resp.msg = "Wrong input! Invalid National ID";
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);
                    }
                }



                if (CountryId == 2) /*for KSA*/
                {
                    // For Damamam Intregaration 
                    bool CheckInDammam = true;
                    bool OnlyDammam = false;
                    var intpatientMrn = 0;


                    if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    {
                        try
                        {
                            intpatientMrn = Convert.ToInt32(patientMrn);
                        }
                        catch (Exception ex)
                        {
                            resp.status = 0;
                            resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                            return Ok(resp);
                        }
                    }




                    //checked IF PATIENT SELECT ANY OTHER BRANCH
                    if (hospitalId > 0 && hospitalId != 9)
                        CheckInDammam = false;

                    // Check Damam MRN Provided
                    if (intpatientMrn > 0 && hospitalId == 9)
                        OnlyDammam = true;

                    // For Damamam Intregaration 
                    // Ahsan New Chjange for Dammam
                    var userInfo = new List<login_check_modal>();

                    //First Check in Other Branches - Old Logic
                    if (!OnlyDammam)
                    {

                        userInfo = loginDb.login_check(lang, hospitalId, PCell, PatientNationId, intpatientMrn, Source, ref errStatus, ref errMessage, IsEncrypt);
                        if (errStatus == 100)
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            resp.error_type = errStatus.ToString();
                            return Ok(resp);
                        }
                    }

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
                            if (!string.IsNullOrEmpty(PatientNationId))
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
                                    TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                                }
                                var FirstChar = TempPcell.Substring(0, 1);
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
                        _damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
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
                            _loginApiCaller.GenerateOTP_V3(userInfo[0].BranchId, userInfo[0].PatientCellNo2, userInfo[0].Registrationno, userInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


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
                else if (CountryId == 3) /*for UAE*/
                {
                    var IdType = "";
                    var IdValue = "";
                    var BranchName = "";

                    if (!string.IsNullOrEmpty(patientMrn.ToString()))
                    {
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(PatientNationId))
                        {
                            IdType = "IdCard";
                            IdValue = PatientNationId.ToString();
                        }
                        else if (!string.IsNullOrEmpty(PCell))
                        {
                            IdType = "Mobile";

                            var TempPcell = PCell.ToString();

                            // change the the Format to
                            TempPcell = TempPcell.Replace("%2B971", "");
                            TempPcell = TempPcell.Replace("%2B", "");
                            TempPcell = TempPcell.Replace("+", "");

                            if (TempPcell.Substring(0, 5) == "00971")
                            {
                                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                            }
                            if (TempPcell.Substring(0, 3) == "971")
                            {
                                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                            }
                            var FirstChar = TempPcell.Substring(0, 1);

                            IdValue = TempPcell.ToString();
                        }
                    }

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _UAEserInfo = _UAEApiCaller.ValidateLoginUserByApi_NewUAE(lang, IdValue, IdType, hospitalId, ref errStatus, ref errMessage);


                    if (_UAEserInfo == null)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);
                    }

                    if (_UAEserInfo.Count == 0)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);

                    }
                    else if (_UAEserInfo.Count == 1)
                    {
                        resp.status = 1;
                        // SENT OTP 
                        int activationCode = 0;
                        int ErrorCode = 0;
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        _loginApiCaller.GenerateOTP_V3(_UAEserInfo[0].BranchId, _UAEserInfo[0].PatientCellNo2, _UAEserInfo[0].Registrationno, _UAEserInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


                        // Encrpt the Data here For condition if 1 record Found
                        var PhoneNumber = _UAEserInfo[0].PatientCellNo2;
                        _UAEserInfo[0].PatientCellNo2 = "";
                        OTP = activationCode.ToString();
                        string MsgContent = "";
                        //OTP = "1111";


                        //var CBC = new CommonDB();
                        //CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);






                        //if (OTP != "6465" && OTP != "1122")

                        // UAE SMS API CAll 
                        //PhoneNumber = "0581178188";
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText_UAE"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                        //Util.SendTestSMS(PhoneNumber, MsgContent);
                        var CBC = new CommonDB();
                        CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);

                        errStatus = 1; // Multiple Record Found
                    }
                    else
                    {
                        // Multiple Record Found 
                        resp.status = 2;
                        errStatus = 2; // Multiple Record Found

                    }
                    // Encrpt the Data here For condition before sending to API
                    var Final_userInfo = new List<login_check_modal>();
                    //Final_userInfo = userInfo;
                    if (IsEncrypt)
                        Final_userInfo = loginDb.Encrpt_UserList_Obj(_UAEserInfo);
                    else
                        Final_userInfo = _UAEserInfo;

                    resp.msg = errMessage;
                    resp.response = Final_userInfo;
                }
                else if (CountryId == 4)
				{

                    var IdType = "";
                    var IdValue = "";

                    if (!string.IsNullOrEmpty(patientMrn.ToString()))
                    {
                        IdType = "patient_reg_no";
                        IdValue = patientMrn.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(PatientNationId))
                        {
                            IdType = "patient_national_id";
                            IdValue = PatientNationId.ToString();
                        }
                        else if (!string.IsNullOrEmpty(PCell))
                        {
                            IdType = "patient_phone";

                            var TempPcell = PCell.ToString();
                            IdValue = TempPcell.ToString();
                        }
                    }

                    ApiCallerEygpt _EYGApiCaller = new ApiCallerEygpt();
                    var _EYGuserInfo = _EYGApiCaller.ValidateLoginUserByApi_EYGPT(lang, IdValue, IdType, hospitalId, ref errStatus, ref errMessage);


                    if (_EYGuserInfo == null)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);
                    }


                    if (_EYGuserInfo.Count == 0)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        return Ok(resp);

                    }
                    else if (_EYGuserInfo.Count == 1)
                    {
                        resp.status = 1;
                        // SENT OTP 
                        int activationCode = 0;
                        int ErrorCode = 0;
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        _loginApiCaller.GenerateOTP_V3(_EYGuserInfo[0].BranchId, _EYGuserInfo[0].PatientCellNo2, _EYGuserInfo[0].Registrationno, _EYGuserInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage);


                        // Encrpt the Data here For condition if 1 record Found
                        var PhoneNumber = _EYGuserInfo[0].PatientCellNo2;
                        _EYGuserInfo[0].PatientCellNo2 = "";
                        OTP = activationCode.ToString();
                        string MsgContent = "";

                        

                        // UAE SMS API CAll 
                        //PhoneNumber = "0581178188";
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText_UAE"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                        //Util.SendTestSMS(PhoneNumber, MsgContent);
                        var CBC = new CommonDB();
                        CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);

                        errStatus = 1; // Multiple Record Found
                    }
                    else
                    {
                        // Multiple Record Found 
                        resp.status = 2;
                        errStatus = 2; // Multiple Record Found

                    }
                    // Encrpt the Data here For condition before sending to API
                    var Final_userInfo = new List<login_check_modal>();
                    //Final_userInfo = userInfo;
                    if (IsEncrypt)
                        Final_userInfo = loginDb.Encrpt_UserList_Obj(_EYGuserInfo);
                    else
                        Final_userInfo = _EYGuserInfo;

                    resp.msg = errMessage;
                    resp.response = Final_userInfo;




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
        [Route("v4/login-pwd")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientLogin_Username_pwd_v4(FormDataCollection col)
        {
            var resp = new GenericResponse();

            var lang = "EN";

            if (!string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_pwd"]))
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var PatientNationId = col["patient_national_id"];
                var PatientPwd = col["patient_pwd"];
                var loginDb = new Login2DB();
                var errStatus = 0;
                var errMessage = "";           
         

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                var userPwdInfo = loginDb.Validate_Patient_PWD_V4_SP(lang, PatientPwd, PatientNationId, ref errStatus, ref errMessage);


                if (errStatus != 0)
                {

                    UserInfo_New userInfo = new UserInfo_New();
                    if (userPwdInfo.BranchID >= 301 && userPwdInfo.BranchID < 400) /*for UAE BRANCHES*/
                    {
                        try
                        {
                            ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                            var IdType = "";
                            var IdValue = "";
                            IdType = "MRN";
                            IdValue = userPwdInfo.RegId;
                            userInfo = _UAEApiCaller.GetPatientDataByApi_NewUAE(lang, IdValue, IdType, userPwdInfo.BranchID, ref errStatus, ref errMessage);
                        }
                        catch
                        {
                            errStatus = 1;
                            errMessage = "No Record Found. Please Try Again Later.";
                        }

                    }
                    else if (userPwdInfo.BranchID == 9)
                    {
                        var apiCaller = new PatientApiCaller();
                        var IdType = "";
                        var IdValue = "";
                        IdType = "MRN";
                        IdValue = userPwdInfo.RegId;

                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                    }
                    else
                    {
                        userInfo = loginDb.ValidateLoginUser_New(lang, userPwdInfo.BranchID, null, userPwdInfo.RegId, null, ref errStatus, ref errMessage, ApiSource);
                    }



                    if (userInfo != null && userInfo.registration_no != null)
					{
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = userInfo;
                    }
                    else
					{
                        resp.status = 0;
                        resp.msg = "Please try again , or use other login options";
                    }
                    
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
        [Route("v4/verify-otp")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerificationCode_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["country_ID"])
                )
            {
                // Ahsan testing Change only Given MRN on Verification
                var loginDb = new Login2DB();

                var lang = "EN";
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var Countryid = Convert.ToInt32(col["country_ID"]);
                var patientMrn = col["patient_reg_no"];
                var patientPhone = col["patient_phone"];
                var patientNationalId = col["patient_national_id"];
                var verificationCode = col["verification_code"];


                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var errStatus = 0;
                var errMessage = "";

                //UserInfo userInfo = loginDb.ValidateLoginUser(lang, hospitalId, null, patientMrn, patientNationalId, ref activationNo, ref errStatus, ref errMessage);
                var userInfo = new UserInfo_New();
                if (hospitalId == 9)
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
                    if (Countryid == 2) /*for KSa BRANCHES*/
					{
                        userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, patientMrn, patientNationalId, ref errStatus, ref errMessage);
                    }
                    else if (Countryid == 3) /*for UAE BRANCHES*/
                    {
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var IdType = "";
                        var IdValue = "";
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                        userInfo = _UAEApiCaller.GetPatientDataByApi_NewUAE(lang, IdValue, IdType,hospitalId, ref errStatus, ref errMessage);
                    }

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
                    //resp.error_type = "invalid_or_mismatched_otp";
                    //resp.msg = "OTP Not Verified V3 - 1";
                    //resp.status = 0;
                    if (verifiedCode == "EXPIRED")
                    {
                        resp.error_type = "Exceed Limit";
                        resp.msg = "Verification Limit Exceed, Please Login Again!";
                        resp.status = 0;


                    }
                    else
                    {
                        resp.error_type = "invalid_or_mismatched_otp";
                        resp.msg = "OTP Not Verified....";
                        resp.status = 0;

                    }
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
                    resp.msg = "OTP Not Verified..";
                    resp.status = 0;
                }
            }
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified.";
                resp.status = 0;
            }

            return Ok(resp);
        }


        public RegistrationData_PatientAdd OpenPatientFile (RegisterPatientUAE PatientData)
		{
            var ObjectReturn = new RegistrationData_PatientAdd();

            var CountryID = PatientData.CountryId;

            var PatientMRN = "";

            if (CountryID == 3) /*for UAE*/
            {
                var registerPatientUAE = new RegisterPatientUAE();
                registerPatientUAE.CurrentCity = PatientData.CurrentCity;
                registerPatientUAE.HospitaId = PatientData.HospitaId;
                registerPatientUAE.IdExpiry = PatientData.IdExpiry;
                registerPatientUAE.IdType = PatientData.IdType;
                registerPatientUAE.PatientAddress = PatientData.PatientAddress;
                registerPatientUAE.PatientBirthday = PatientData.PatientBirthday;
                registerPatientUAE.PatientEmail = PatientData.PatientEmail;
                registerPatientUAE.PatientFamilyName = PatientData.PatientFamilyName;
                registerPatientUAE.PatientFirstName = PatientData.PatientFirstName;
                registerPatientUAE.PatientGender = PatientData.PatientGender;

                if (!String.IsNullOrEmpty(PatientData.PatientId))
				{
                    registerPatientUAE.PatientId = PatientData.PatientId.ToString();
                }
                else
				{
                    registerPatientUAE.PatientId = null;

                }
                

                registerPatientUAE.PatientLastName = PatientData.PatientLastName;
                registerPatientUAE.PatientMaritalStatusId = PatientData.PatientMaritalStatusId;
                registerPatientUAE.PatientMiddleName = PatientData.PatientMiddleName;
                registerPatientUAE.PatientNationalId = PatientData.PatientNationalId;
                registerPatientUAE.PatientNationalityId = PatientData.PatientNationalityId;
                registerPatientUAE.PatientPhone = PatientData.PatientPhone;
                registerPatientUAE.PatientTitleId = PatientData.PatientTitleId;
                registerPatientUAE.skipDuplicateCheck = false;

                ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                RegistrationPostResponse ReturnObject;
                var APiResilts = _UAEApiCaller.PatientAddApi_NewUAE(registerPatientUAE, out ReturnObject);

                if (APiResilts && !string.IsNullOrEmpty(ReturnObject.Mrn))
                {   
                    PatientMRN = ReturnObject.Mrn;
                }
                else
                {
                    return null;
                }


            }
            else  // Currently For KSA
            {
                if (PatientData.HospitaId == 9)
                {   
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    PostResponse_AddPatient ReturnObject;
                    var APiResilts = _loginApiCaller.PatientAddApi_NewDammam_ForRegistration(PatientData, out ReturnObject);

                    if (APiResilts)
                    {
                        PatientMRN = ReturnObject.data.mrn;
                    }
                    else
					{
                        return null;
					}                    
                }
                else
                {
                    var patientDb = new PatientDB();
                    var NEwIntregistrationNo = 0;
                    var status = 0;
                    var msg = "";
                    var successType = "";                    
                    var activationNo = 0;
                    var errorType = "";

                    var registerPatientResFailure = patientDb.RegisterNewPatient_V5(PatientData, ref status, ref msg, ref errorType, ref successType, ref NEwIntregistrationNo, ref activationNo, "MobileApp");
                                       
                    
                    if (status != 1)
                    {
                        return null;
                    }
                    else
					{
                        PatientMRN = NEwIntregistrationNo.ToString();
                    }
                }

            }

            if (string.IsNullOrEmpty(PatientMRN))
			{
                return null;
			}

            ObjectReturn.BranchID = PatientData.HospitaId;
            ObjectReturn.PatientNationalID = PatientData.PatientNationalId;
            ObjectReturn.PatientPhone = PatientData.PatientPhone;
            ObjectReturn.RegistrationID = PatientMRN;

            return ObjectReturn;
        }

        [HttpPost]
        [Route("v5/verify-otp")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerificationCode_V5(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["country_ID"])
                && !string.IsNullOrEmpty(col["Reason_Code"])
                )
            {
                // Ahsan testing Change only Given MRN on Verification
                var loginDb = new Login2DB();

                var lang = "EN";
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var Countryid = Convert.ToInt32(col["country_ID"]);
                var patientMrn = col["patient_reg_no"];
                var patientPhone = col["patient_phone"];
                var patientNationalId = col["patient_national_id"];
                var verificationCode = col["verification_code"];

                var CodeReason = Convert.ToInt32(col["Reason_Code"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var errStatus = 0;
                var errMessage = "";
                var IsVerified = false;
                var patientDb = new PatientDB();

                if (CodeReason == 6)
				{
                    // Validate the OTP First then Create the Patient File in Respective Branch
                    var verifiedCode2 = patientDb.VerifyOTP_ForRegistation(hospitalId, patientMrn, patientPhone, verificationCode , 6);
                    if ((verifiedCode2 != null && verifiedCode2 == verificationCode))
                    {
                        // Now Create the Patient File in the Branch
                        IsVerified = true;
                        var PatientRegistrationData =  patientDb.Add_PatientFile(patientMrn);

                        if (PatientRegistrationData != null)
						{
                            var PData = OpenPatientFile(PatientRegistrationData);

                            if (PData != null)
							{
                                hospitalId = PData.BranchID;
                                patientMrn = PData.RegistrationID;
                                patientNationalId = PData.PatientNationalID;
                            }
                            else
							{
                                resp.msg = "Failed, To Create File! Please try to Open File Again";
                                resp.status = 0;
                                return Ok(resp);
                            }

                            
                        }
                        else
						{
                            resp.msg = "Failed, NO Data Found! Please try to Open File Again";
                            resp.status = 0;
                            return Ok(resp);
                        }                        
                    }

                }

                //UserInfo userInfo = loginDb.ValidateLoginUser(lang, hospitalId, null, patientMrn, patientNationalId, ref activationNo, ref errStatus, ref errMessage);
                var userInfo = new UserInfo_New();
                if (hospitalId == 9)
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
                    if (Countryid == 2) /*for KSa BRANCHES*/
                    {
                        userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, patientMrn, patientNationalId, ref errStatus, ref errMessage);
                    }
                    else if (Countryid == 3) /*for UAE BRANCHES*/
                    {
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var IdType = "";
                        var IdValue = "";
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                        userInfo = _UAEApiCaller.GetPatientDataByApi_NewUAE(lang, IdValue, IdType, hospitalId, ref errStatus, ref errMessage);
                    }

                }



                
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

                var verifiedCode = "";
                if (!IsVerified)
                    verifiedCode  = patientDb.VerifyOTP(hospitalId, patientMrn, patientPhone, verificationCode);

                if ((verifiedCode != null && verifiedCode == verificationCode ) || IsVerified)
                {

                    //var dt = patientDb.GetPatientDataDT_V2(lang, hospitalId, Convert.ToInt32 (patientMrn), ref errStatus, ref errMessage);
                    resp.response = userInfo;
                    resp.msg = "OTP Verified";
                    resp.status = 1;
                }
                else
                {
                    //resp.error_type = "invalid_or_mismatched_otp";
                    //resp.msg = "OTP Not Verified V3 - 1";
                    //resp.status = 0;
                    if (verifiedCode == "EXPIRED")
                    {
                        resp.error_type = "Exceed Limit";
                        resp.msg = "Verification Limit Exceed, Please Login Again!";
                        resp.status = 0;


                    }
                    else
                    {
                        resp.error_type = "invalid_or_mismatched_otp";
                        resp.msg = "OTP Not Verified....";
                        resp.status = 0;

                    }
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
                    resp.msg = "OTP Not Verified..";
                    resp.status = 0;
                }
            }
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified.";
                resp.status = 0;
            }

            return Ok(resp);
        }




        //     [HttpPost]
        //     [Route("v4/UAE-SMS")]
        //     [ResponseType(typeof(List<GenericResponse>))]
        //     public IHttpActionResult TEST_UAE_SMS(FormDataCollection col)
        //     {
        //         var resp = new GenericResponse();
        //         if (!string.IsNullOrEmpty(col["mobile"]) && !string.IsNullOrEmpty(col["sms_text"])
        //             )
        //{

        //             var mobileNumber = Convert.ToInt64(col["mobile"]);
        //             var smsText = col["sms_text"].ToString();

        //	//var sms1 = (new SmsApi().SendSms(mobileNumber, smsText));

        //	//resp.error_type = sms1.ErrorMessage.ToString();
        //	//resp.msg = sms1.Response.ToString();
        //	//if (sms1.ErrorFlag)
        //	//	resp.status = 1;
        //	//else
        //	//	resp.status = 0;

        //}
        //else
        //         {
        //             resp.error_type = "Missing Parameters";
        //             resp.msg = "Missing Parameters.";
        //             resp.status = 0;
        //         }

        //         return Ok(resp);
        //     }



        [HttpPost]
        [Route("v4/UpdateData-OTP")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult VerifyOTP_And_UpdateData(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_DOB"]) && !string.IsNullOrEmpty(col["patient_Gender"]) && !string.IsNullOrEmpty(col["Marital_Status"])
                )
            {
                //&& !string.IsNullOrEmpty(col["patient_nationality_id"])
                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var PatientPhone = col["patient_phone"];
                var patient_DOB = col["patient_DOB"];
                var ActivationCode = col["verification_code"];
                var patient_Gender = col["patient_Gender"];
                var Marital_Status = col["Marital_Status"];
                //var PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                var PatientNationalityId = 0;
                var EMail = col["patient_Email"];

                var patient_Gender_i = 1;
                var Status = 0;
                var Msg = "";
                var Sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();

                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];

                if (!string.IsNullOrEmpty(col["Source"]))
                    Sources = col["Source"];

                
                // AHSAN ADDING 14-07-2024
                patient_DOB = patient_DOB.Replace("%3A", ":");


                DateTime PDateOfBirth;
                if (!DateTime.TryParse(patient_DOB, out PDateOfBirth))
                {
                    // handle parse failure
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Date Format";
                    return Ok(_resp);
                }
                if (PDateOfBirth > DateTime.Now)
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Date";
                    return Ok(_resp);
                }

                if (patient_Gender != "F" && patient_Gender != "M")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Gender Format";
                    return Ok(_resp);
                }

                if (Marital_Status != "S" && Marital_Status != "M")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Marital Status Format";
                    return Ok(_resp);
                }
                var iMarital_Status = 6;

                if (Marital_Status == "S")
                    iMarital_Status = 6;

                if (Marital_Status == "M")
                    iMarital_Status = 2;



                if (CDB.VerifyOTP_Mobile(hospitaId, PatientPhone, ActivationCode))
                {
                    if (hospitaId >= 301 && hospitaId < 400)
                    {
                        int errStatus = 0;
                        string errMessage = "";

                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var _NewData = _UAEApiCaller.UpdatePatientBasicData_NewUAE(hospitaId, CMRN, PDateOfBirth.ToString("dd-MM-yyyy"), Marital_Status, patient_Gender, PatientPhone, EMail, ref errStatus, ref errMessage);

                        _resp.status = errStatus;
                        _resp.msg = errMessage;
                        return Ok(_resp);
                    }
                    else
					{
                        PatientDB patientDb = new PatientDB();
                        if (patient_Gender == "F")
                            patient_Gender_i = 1;
                        else
                            patient_Gender_i = 2;
                        var RegistrationNo = Convert.ToInt32(CMRN);

                        patientDb.UpdatePatientData(hospitaId, RegistrationNo, PDateOfBirth, patient_Gender_i, PatientPhone, iMarital_Status, PatientNationalityId, EMail, Sources, ref Status, ref Msg);
                        _resp.status = Status;
                        _resp.msg = Msg;
                    }
                    
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! :  Wrong OTP ";
                }



                //if (Error_Code == 0)
                //    _resp.status = 1;
                //else
                //    _resp.status = 0;

                //_resp.msg = MSG;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        [HttpPost]
        [Route("v4/Login-Mobile")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult LoginMobile_Request_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();

            var lang = "EN";
            //var hospitalId = 0;
            //var patientMrn = "";

            var IsEncrypt = true;
            if (string.IsNullOrEmpty(col["country_ID"]))
            {
                resp.status = 0;
                resp.msg = "Missing Parameter! Country Required";
                return Ok(resp);
            }

            if (!string.IsNullOrEmpty(col["patient_phone"]))
            {
                var Source = "";
                var CountryId = 0;
                try
                {
                    CountryId = Convert.ToInt32(col["country_ID"]);
                }
                catch (Exception ex)
                {
                    resp.status = 0;
                    resp.msg = "Wrong Prameter. Please Enter the Valid Input.";
                    return Ok(resp);
                }



                if (!string.IsNullOrEmpty(col["Source"]))
                    Source = col["Source"];

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["IsEncrypt"]))
                {
                    if (col["IsEncrypt"] == "0")
                        IsEncrypt = false;
                }

                
                var PCell = col["patient_phone"];

                var loginDb = new Login2DB();
                var errStatus = 0;
                var errMessage = "";
                var OTP = "";                

                if (CountryId == 2) /*for KSA*/
                {
                    // For Damamam Intregaration 
                    
                    var intpatientMrn = 0;
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();

                    // For Damamam Intregaration 
                    // Ahsan New Chjange for Dammam
                    var userInfo = new List<login_check_modal>();

                    userInfo = loginDb.login_check(lang, 0, PCell,null , intpatientMrn, Source, ref errStatus, ref errMessage, IsEncrypt);
                    if (errStatus == 100)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        resp.error_type = errStatus.ToString();
                        return Ok(resp);
                    }

                    if (userInfo.Count > 0)
					{
                        errStatus = 1; // Multiple Record Found
                        resp.status = 1;
                        int activationCode = 0, ErrorCode;
                        _loginApiCaller.GenerateOTP_V3("0", userInfo[0].PatientCellNo2, "", "", Source, ref activationCode, ref errStatus, ref errMessage, 2);

                        var PhoneNumber = userInfo[0].PatientCellNo2;
                        userInfo[0].PatientCellNo2 = "";
                        OTP = activationCode.ToString();
                        string MsgContent = "";
                        if (OTP != "6465" && OTP != "1122")
                        {
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                            Util.SendTestSMS(PhoneNumber, MsgContent);
                        }

                        resp.status = 1;
                        resp.msg = "OTP has been sent to register mobile Number.";
                        return Ok(resp);
                    }

                    // Call dammam API Function fill list
                    
                    List<login_check_modal> _damuserInfo = new List<login_check_modal>();

                    if (true)
                    {
                        var IdType = "";
                        var IdValue = "";                        
                            
                        if (!string.IsNullOrEmpty(PCell))
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
                                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                            }
                            var FirstChar = TempPcell.Substring(0, 1);
                            if (FirstChar != "0")
                            {
                                TempPcell = "0" + TempPcell;
                            }


                            IdValue = TempPcell.ToString();
                            // Call dammam API Function fill list
                            //LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            //List<login_check_modal> _damuserInfo;
                            _damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                        }

                    }

                    int Tempcount1 = userInfo.Count;
                    userInfo.AddRange(_damuserInfo);
                    int Tempcount2 = userInfo.Count;
                    errStatus = 1; // SET initial Error No record Found
                                   // Now check Count 
                    if (userInfo.Count > 0)
                    {
                        errStatus = 1; // Multiple Record Found
                        resp.status = 1;
                        int activationCode = 0, ErrorCode;
                        _loginApiCaller.GenerateOTP_V3(userInfo[0].BranchId, userInfo[0].PatientCellNo2, userInfo[0].Registrationno, userInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage, 2);

                        var PhoneNumber = userInfo[0].PatientCellNo2;
                        userInfo[0].PatientCellNo2 = "";
                        OTP = activationCode.ToString();
                        string MsgContent = "";
                        if (OTP != "6465" && OTP != "1122")
                        {
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                            Util.SendTestSMS(PhoneNumber, MsgContent);
                        }

                        resp.status = 1;
                        resp.msg = "OTP has been sent to register mobile Number.";
                        return Ok(resp);
                    }
                    else
					{
                        resp.status = 0;
                        resp.msg = "No record Found.";
                        return Ok(resp);
                    }
                }
                else if (CountryId == 3) /*for UAE*/
                {
                    var IdType = "";
                    var IdValue = "";
                    var BranchName = "";
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                    if (!string.IsNullOrEmpty(PCell))
                    {
                        IdType = "Mobile";

                        var TempPcell = PCell.ToString();

                        // change the the Format to
                        TempPcell = TempPcell.Replace("%2B971", "");
                        TempPcell = TempPcell.Replace("%2B", "");
                        TempPcell = TempPcell.Replace("+", "");

                        if (TempPcell.Substring(0, 5) == "00971")
                        {
                            TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                        }
                        if (TempPcell.Substring(0, 3) == "971")
                        {
                            TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                        }
                        var FirstChar = TempPcell.Substring(0, 1);
                        //if (FirstChar != "0")
                        //{
                        //    TempPcell = "0" + TempPcell;
                        //}


                        IdValue = TempPcell.ToString();
                        var _UAEserInfo = _UAEApiCaller.ValidateLoginUserByApi_NewUAE(lang, IdValue, IdType,0, ref errStatus, ref errMessage);
                        if (_UAEserInfo.Count == 0)
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            return Ok(resp);

                        }
                        else 
                        {
                            resp.status = 1;
                            // SENT OTP 
                            int activationCode = 0;
                            int ErrorCode = 0;
                            LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            _loginApiCaller.GenerateOTP_V3(_UAEserInfo[0].BranchId, _UAEserInfo[0].PatientCellNo2, _UAEserInfo[0].Registrationno, _UAEserInfo[0].PatientId, Source, ref activationCode, ref errStatus, ref errMessage , 3);


                            // Encrpt the Data here For condition if 1 record Found
                            var PhoneNumber = _UAEserInfo[0].PatientCellNo2;
                            _UAEserInfo[0].PatientCellNo2 = "";
                            OTP = activationCode.ToString();
                            string MsgContent = "";
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText_UAE"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                            
                            var CBC = new CommonDB();
                            CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);

                            errStatus = 1; // Multiple Record Found

                            resp.status = 1;
                            resp.msg = "OTP has been send ";
                            return Ok(resp);



                        }

                        resp.status = 0;
                        resp.msg = "No record found";
                        return Ok(resp);

                    }



                    resp.status = 0;
                    resp.msg = "No record found";
                    return Ok(resp);
                }
                resp.status = 0;
                resp.msg = "No record Found.";
                return Ok(resp);
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }


            return Ok(resp);
        }




        [HttpPost]
        [Route("v4/Validate-OTP-Mobile")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerificationCode_Mobile_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_phone"])
                && !string.IsNullOrEmpty(col["country_ID"])
                )
            {
                var loginDb = new Login2DB();
                var lang = "EN";                
                var Countryid = Convert.ToInt32(col["country_ID"]);                
                var patientPhone = col["patient_phone"];                
                var verificationCode = col["verification_code"];

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var errStatus = 0;
                var errMessage = "";

                if (Countryid  == 3)
				{
                    var TempPcell = patientPhone;
                    // change the the Format to
                    TempPcell = TempPcell.Replace("%2B971", "");
                    TempPcell = TempPcell.Replace("%2B", "");
                    TempPcell = TempPcell.Replace("+", "");

                    if (TempPcell.Substring(0, 5) == "00971")
                    {
                        TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                    }
                    if (TempPcell.Substring(0, 3) == "971")
                    {
                        TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                    }
                    var FirstChar = TempPcell.Substring(0, 1);
                    //if (FirstChar != "0")
                    //{
                    //    TempPcell = "0" + TempPcell;
                    //}


                    patientPhone = TempPcell.ToString();
                }

                var patientDb = new PatientDB();
                var verifiedCode = patientDb.VerifyOTP_Mobile( patientPhone, verificationCode , Countryid);


                if (verifiedCode != null && verifiedCode == verificationCode)
				{
                    if (Countryid == 2) /*for KSA*/
                    {
                        //// For Damamam Intregaration 
                        //bool CheckInDammam = true;
                        //bool OnlyDammam = false;
                        //var intpatientMrn = 0;


                        // For Damamam Intregaration 
                        // Ahsan New Chjange for Dammam
                        var userInfo = new List<login_check_modal>();

                        userInfo = loginDb.login_check(lang, 0, patientPhone,null , 0, "MobileAPP", ref errStatus, ref errMessage, true);                        

                        // Call dammam API Function fill list
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        List<login_check_modal> _damuserInfo = new List<login_check_modal>();

                        var IdType = "";
                        var IdValue = "";


						// nTEMP AHSAN Comment as not working from My PC
						if (!string.IsNullOrEmpty(patientPhone))
						{
							IdType = "MOB";

							var TempPcell = patientPhone.ToString();

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
								TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
							}
							var FirstChar = TempPcell.Substring(0, 1);
							if (FirstChar != "0")
							{
								TempPcell = "0" + TempPcell;
							}


							IdValue = TempPcell.ToString();

							_damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
						}


						int Tempcount1 = userInfo.Count;
                        userInfo.AddRange(_damuserInfo);
                        int Tempcount2 = userInfo.Count;
                        errStatus = 1;

                        // Encrpt the Data here For condition before sending to API
                        var Final_userInfo = new List<login_check_modal>();
                        Final_userInfo = userInfo;
                        resp.msg = errMessage;
                        resp.response = Final_userInfo;
                        resp.status = 1;
                        return Ok(resp);

                    }
                    else if (Countryid == 3) /*for UAE*/
                    {
                        var IdType = "";
                        var IdValue = "";
                        var BranchName = "";
                        
                        
                            
                        if (!string.IsNullOrEmpty(patientPhone))
                        {
                            IdType = "Mobile";

                            var TempPcell = patientPhone.ToString();

                            // change the the Format to
                            TempPcell = TempPcell.Replace("%2B971", "");
                            TempPcell = TempPcell.Replace("%2B", "");
                            TempPcell = TempPcell.Replace("+", "");

                            if (TempPcell.Substring(0, 5) == "00971")
                            {
                                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
                            }
                            if (TempPcell.Substring(0, 3) == "971")
                            {
                                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
                            }
                            var FirstChar = TempPcell.Substring(0, 1);
                                

                            IdValue = TempPcell.ToString();

                            ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                            var _UAEserInfo = _UAEApiCaller.ValidateLoginUserByApi_NewUAE(lang, IdValue, IdType, 0, ref errStatus, ref errMessage);

                            var Final_userInfo = new List<login_check_modal>();
                            
                            //if (IsEncrypt)
                            //    Final_userInfo = loginDb.Encrpt_UserList_Obj(_UAEserInfo);
                            //else
                                Final_userInfo = _UAEserInfo;
                            resp.status = 1;
                            resp.msg = errMessage;
                            resp.response = Final_userInfo;
                            return Ok(resp);

                        }
                    }

                }
                else
				{
                    if (verifiedCode == "EXPIRED")
                    {
                        resp.error_type = "Exceed Limit";
                        resp.msg = "Verification Limit Exceed, Please Login Again!";
                        resp.status = 0;


                    }
                    else
                    {
                        resp.error_type = "invalid_or_mismatched_otp";
                        resp.msg = "OTP Not Verified....";
                        resp.status = 0;

                    }
                    //resp.error_type = "invalid_or_mismatched_otp";
                    //resp.msg = "OTP Not Verified..";
                    //resp.status = 0;
                    //return Ok(resp);
                }
            }            
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified.";
                resp.status = 0;
            }

            return Ok(resp);
        }



        [HttpPost]
        [Route("v4/Resend-otp-Mobile")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Resend_OTP_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {

                var lang = "EN";
                var hospitalId = 0;
                if (!string.IsNullOrEmpty(col["country_ID"]) && !string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];
                    
                    var PhoneNumber = col["patient_phone"];
                    var CountryID  = Convert.ToInt32( col["country_ID"]); 




                    var loginDb = new Login2DB();
                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    var OTP = "";


                    errStatus = 1; // Multiple Record Found
                    resp.status = 1;
                    int activationCode = 0, ErrorCode;

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    _loginApiCaller.GenerateOTP_V3("0", PhoneNumber, "", "", "MobileApp", ref activationCode, ref errStatus, ref errMessage, CountryID);

                    //var PhoneNumber = userInfo[0].PatientCellNo2;
                    //userInfo[0].PatientCellNo2 = "";
                    OTP = activationCode.ToString();
                    string MsgContent = "";
                    if (OTP != "6465" && OTP != "1122")
                    {
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                        Util.SendTestSMS(PhoneNumber, MsgContent);
                    }

                    resp.status = 1;
                    resp.msg = "OTP has been sent to register mobile Number.";
                    return Ok(resp);






                    
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {

                //log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v4/UAE-Token")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult UAE_TEST_TOKKEN(FormDataCollection col)
        {
            var resp = new GenericResponse();
            
                var temp = RestUtility.GetTokenTESTING("UAE");
                return Ok(temp); 
        }


    }
}
