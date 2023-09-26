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

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class Login2Controller : ApiController
    {
        private static readonly log4net.ILog log =  log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Validate the patient login credentials.
        /// </summary>
        /// <returns>Return the user info on successfull validation of patient login credentials</returns>
        [HttpPost]
        [Route("login")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Validatepatient_SendOTP(FormDataCollection col)
        {
            try
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var pregno = col["patient_reg_no"];
                var pnationalid = col["patient_national_id"];

                Login2DB _loginDB = new Login2DB();
                LoginApiCaller _loginApiCaller = new LoginApiCaller();
                UserInfo _userInfo;
                
                int errStatus = 0;
                string errMessage = "";
                int activationNo = 0;

                if (!Util.OasisBranches.Contains(hospitaId))
                {
                    _userInfo = _loginDB.ValidateLoginUser(lang, hospitaId, null, pregno, pnationalid, ref activationNo, ref errStatus, ref errMessage);
                }
                else
                {
                    _userInfo = _loginApiCaller.ValidateLoginUserByApi(lang, hospitaId, null, pregno, pnationalid, ref activationNo, ref errStatus, ref errMessage);
                }


                GenericResponse resp = new GenericResponse();


                if (errStatus == 0 && _userInfo != null)
                {
                    //string smsRes = "";

                    if (hospitaId != 201 && !Util.UaeBranches.Contains(hospitaId))
                        Util.SendTestSMS(_userInfo.phone, activationNo + " is your OTP for SGH Mobile App");
                    else if (hospitaId == 201)
                        Util.SendSMS_Cairo(_userInfo.phone, activationNo + " is your OTP for SGH Mobile App");
                    else if (Util.UaeBranches.Contains(hospitaId))
                    {
                        string response = Util.SendSMS_UAE(hospitaId, _userInfo.phone, activationNo + " is your OTP for SGH Mobile App");
                        log.Info("UAE SMS Reponse: " + response);
                    }



                    //resp.activation_num = activationNo;
                    resp.status = 1;
                    resp.msg = errMessage;
                    
                    // No need to send users
                    //resp.response = _userInfo;
                    //resp.response = smsRes;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = errMessage;
                    //resp.response = null;
                    resp.error_type = errStatus.ToString();


                }
                return Ok(resp);
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }

            return Ok();
        }


        ////this one will delete or comment later ahsan 
        //[HttpPost]
        //[Route("login-check")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PatientList_Request(FormDataCollection col)
        //{
        //    var resp = new GenericResponse();
        //    try
        //    {
        //        var lang = "EN";
        //        var hospitalId = 0;
        //        if (!string.IsNullOrEmpty(col["lang"]))                
        //            lang = col["lang"];

                
        //        if (string.IsNullOrEmpty(col["hospital_id"]) )
        //            hospitalId = 0;                
        //        else
        //            hospitalId = Convert.ToInt32(col["hospital_id"]);

        //        var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
        //        var PatientNationId = col["patient_national_id"];
        //        var PCell = col["patient_phone"];

        //        var loginDb = new Login2DB();
        //        var loginApiCaller = new LoginApiCaller();                
        //        var errStatus = 0;
        //        var errMessage = "";
        //        var OTP = "";

        //        if (!string.IsNullOrEmpty(col["patient_reg_no"]))
        //        {
        //            if (string.IsNullOrEmpty(col["hospital_id"]))
        //            {

        //                resp.status = 0;
        //                resp.msg = "Missing Prameter. With MRN Please Hospital ID";
        //                resp.error_type = errStatus.ToString();
        //                return Ok(resp);

        //            }
        //        }

        //        if (!string.IsNullOrEmpty(col["patient_national_id"]))
        //        {
        //            if (PatientNationId.Length !=  10)
        //            {
        //                resp.status = 0;
        //                resp.msg = "Wrong input! Invalid National ID";
        //                resp.error_type = errStatus.ToString();
        //                return Ok(resp);
        //            }
        //        }

        //            var userInfo = loginDb.ValidateLoginUser_List (lang , hospitalId, PCell,  PatientNationId, patientMrn , ref errStatus, ref errMessage , ref OTP);

        //        if (errStatus != 1)
        //        {
        //            resp.status = errStatus;
        //            if (errStatus == 0)
        //            {
                        
        //                resp.status = 1;
        //                string smsRes = "";
        //                DataRow dr = userInfo.Rows[0];
        //                var PhoneNumber = dr["PatientCellNo2"].ToString();
        //                userInfo.Rows[0]["PatientCellNo2"] = "";

        //                //PhoneNumber = "0592285955";

        //                string MsgContent = "";
        //                MsgContent += "<";
        //                MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_InitialWord"].ToString();
        //                MsgContent += "> ";
        //                MsgContent += "Your OTP for SGH Mobile App is "+ OTP;
        //                //MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_Enter"].ToString();
        //                MsgContent += "%0a";
        //                MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_ESignature"].ToString();


        //                //if (PhoneNumber == "0592285955")
        //                //{   
        //                //    Util.SendSMS_Cairo("01221738737", MsgContent);                        
        //                //}


        //                if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
        //                    Util.SendTestSMS(PhoneNumber, MsgContent);
        //                else if (hospitalId == 201)
        //                    Util.SendSMS_Cairo(PhoneNumber, MsgContent);
        //                else if (Util.UaeBranches.Contains(hospitalId))
        //                {
        //                    string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
        //                    log.Info("UAE SMS Reponse: " + response);
        //                }


        //                //if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
        //                //    Util.SendTestSMS(PhoneNumber, OTP + " is your OTP for SGH Mobile App");
        //                //else if (hospitalId == 201)
        //                //    Util.SendSMS_Cairo(PhoneNumber, OTP + " is your OTP for SGH Mobile App");
        //                //else if (Util.UaeBranches.Contains(hospitalId))
        //                //{
        //                //    string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, OTP + " is your OTP for SGH Mobile App");
        //                //    log.Info("UAE SMS Reponse: " + response);
        //                //}



        //            }


        //            resp.msg = errMessage;
        //            resp.response = userInfo;
        //        }
        //        else
        //        {
        //            resp.status = 0;
        //            resp.msg = errMessage;
        //            resp.error_type = errStatus.ToString();
        //        }
        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {

        //        log.Error(ex);
        //    }

        //    return Ok();
        //}


        [HttpPost]
        [Route("login-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_NEW(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                if (string.IsNullOrEmpty(col["Source"]))
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter- Source missing";
                    return Ok(resp);
                }

                var lang = "EN";
                var hospitalId = 0;
                var Source = col["Source"];
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];



                var patientMrn = 0;
                try
                {
                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);

                    patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                }
                catch (Exception e)
                {
                    resp.status = 0;
                    resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(resp);
                }



                var PatientNationId = col["patient_national_id"];
                var PCell = col["patient_phone"];

                var loginDb = new Login2DB();
                var loginApiCaller = new LoginApiCaller();
                var errStatus = 0;
                var errMessage = "";
                var OTP = "";

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    if (string.IsNullOrEmpty(col["hospital_id"]))
                    {

                        resp.status = 0;
                        resp.msg = "Missing Prameter. With MRN Please Hospital ID";
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

                var userInfo = loginDb.ValidateLoginUser_List(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, ref OTP);

                if (errStatus != 1)
                {
                    resp.status = errStatus;
                    if (errStatus == 0)
                    {

                        resp.status = 1;
                        //string smsRes = "";
                        DataRow dr = userInfo.Rows[0];
                        var PhoneNumber = dr["PatientCellNo2"].ToString();
                        userInfo.Rows[0]["PatientCellNo2"] = "";

                        //PhoneNumber = "0592285955";

                        string MsgContent = "";
                        MsgContent += "<";
                        MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_InitialWord"].ToString();
                        MsgContent += "> ";
                        MsgContent += "Your OTP for SGH Mobile App is " + OTP;
                        //MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_Enter"].ToString();
                        MsgContent += "%0a";
                        MsgContent += ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_ESignature"].ToString();


                        //if (PhoneNumber == "0592285955")
                        //{   
                        //    Util.SendSMS_Cairo("01221738737", MsgContent);                        
                        //}


                        if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                            Util.SendTestSMS(PhoneNumber, MsgContent);
                        else if (hospitalId == 201)
                            Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                        else if (Util.UaeBranches.Contains(hospitalId))
                        {
                            string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                            log.Info("UAE SMS Reponse: " + response);
                        }


                        //if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                        //    Util.SendTestSMS(PhoneNumber, OTP + " is your OTP for SGH Mobile App");
                        //else if (hospitalId == 201)
                        //    Util.SendSMS_Cairo(PhoneNumber, OTP + " is your OTP for SGH Mobile App");
                        //else if (Util.UaeBranches.Contains(hospitalId))
                        //{
                        //    string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, OTP + " is your OTP for SGH Mobile App");
                        //    log.Info("UAE SMS Reponse: " + response);
                        //}



                    }


                    resp.msg = errMessage;
                    resp.response = userInfo;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = errMessage;
                    resp.error_type = errStatus.ToString();
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }

            return Ok();
        }



        [HttpPost]
        [Route("v2/login-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_V2(FormDataCollection col)
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
                    catch(Exception ex)
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
                if (CheckInDammam)
				{
                    // Call dammam API Function fill list
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    UserInfo _userInfo;
                    //_userInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, hospitalId, null, pregno, pnationalid, ref activationNo, ref errStatus, ref errMessage);
                }

                if (!OnlyDammam)
                    userInfo = loginDb.login_check(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, ref OTP, IsEncrypt);
                

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

                            var PhoneNumber = userInfo[0].PatientCellNo2;
                            userInfo[0].PatientCellNo2 = "";
                            
                            //PhoneNumber = "0592285955";

                            string MsgContent = "";
                            if (OTP != "6465")
                            {
                                //MsgContent = "<#> SGHC OTP Code " + OTP + " ";
                                MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                                MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();

                                Util.SendTestSMS(PhoneNumber, MsgContent);
                                //if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                                //    Util.SendTestSMS(PhoneNumber, MsgContent);
                                //else if (hospitalId == 201)
                                //    Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                                //else if (Util.UaeBranches.Contains(hospitalId))
                                //{
                                //    string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                                //    log.Info("UAE SMS Reponse: " + response);
                                //}
                            }
                           
                        }


                        resp.msg = errMessage;
                        resp.response = userInfo;
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
        [Route("v2/login-check-ency")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_ency(FormDataCollection col)
        {
            //Util.SendSMS_Cairo("01221738737", "asdfadsfasd");

            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                if (string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalId = 0;
                else
                    hospitalId = Convert.ToInt32(col["hospital_id"]);

                var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                var PatientNationId = col["patient_national_id"];
                var PCell = col["patient_phone"];

                var loginDb = new Login2DB();
                var loginApiCaller = new LoginApiCaller();
                var errStatus = 0;
                var errMessage = "";
                var OTP = "";

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    if (string.IsNullOrEmpty(col["hospital_id"]))
                    {

                        resp.status = 0;
                        resp.msg = "Missing Prameter. With MRN Please Hospital ID";
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

                var userInfo = loginDb.ValidateLoginUser_List_Encypt(lang, hospitalId, PCell, PatientNationId, patientMrn, ref errStatus, ref errMessage, ref OTP);

                if (errStatus != 1)
                {
                    resp.status = errStatus;
                    if (errStatus == 0)
                    {

                        var PhoneNumber = "";
                        //resp.status = 1;
                        //string smsRes = "";
                        //DataRow dr = userInfo.Rows[0];
                        //var PhoneNumber = dr["PatientCellNo2"].ToString();
                        //userInfo.Rows[0]["PatientCellNo2"] = "";

                        
                        //<

                        string MsgContent = "";

                        //MsgContent = "<#> SGHC OTP Code " + OTP + " ";
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();


                        if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                            Util.SendTestSMS(PhoneNumber, MsgContent);
                        else if (hospitalId == 201)
                            Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                        else if (Util.UaeBranches.Contains(hospitalId))
                        {
                            string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                            log.Info("UAE SMS Reponse: " + response);
                        }



                    }


                    resp.msg = errMessage;
                    resp.response = userInfo;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = errMessage;
                    resp.error_type = errStatus.ToString();
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/login-pwd")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientLogin_Username_pwd(FormDataCollection col)
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

                var userInfo = loginDb.ValidatePatientPassword (lang, PatientPwd, PatientNationId,  ref errStatus, ref errMessage);
                           
                if (errStatus != 0)
                {
                    resp.status = errStatus;
                    resp.msg = errMessage;
                    resp.response = userInfo;
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
        [Route("verify-otp")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerificationCode(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["verification_code"]) && (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"])))
            {
                // Ahsan testing Change only Given MRN on Verification
                var loginDb = new Login2DB();
                var loginApiCaller = new LoginApiCaller();

                var lang = "EN";
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = col["patient_reg_no"];
                var patientPhone = col["patient_phone"];
                var patientNationalId = col["patient_national_id"];
                var verificationCode = col["verification_code"];

                var activationNo = 0;
                var errStatus = 0;
                var errMessage = "";
                
                UserInfo userInfo = !Util.OasisBranches.Contains(hospitalId) ? loginDb.ValidateLoginUser(lang, hospitalId, null, patientMrn, patientNationalId, ref activationNo, ref errStatus, ref errMessage) : loginApiCaller.ValidateLoginUserByApi(lang, hospitalId, null, patientMrn, patientNationalId, ref activationNo, ref errStatus, ref errMessage);

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
                
                var patientDb = new PatientDB();
                var verifiedCode = patientDb.VerifyOTP(hospitalId, patientMrn, patientPhone, verificationCode);

                if (verifiedCode != null && verifiedCode == verificationCode)
                {
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
            else
            {
                resp.error_type = "Missing Parameters";
                resp.msg = "OTP Not Verified";
                resp.status = 0;
            }
            
            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/verify-otp")]
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

                var userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, patientMrn, patientNationalId,ref errStatus, ref errMessage);
                

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
            else if(!string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_phone"]))
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
                    
                    var userInfo = patientDb.GetPatientList_ByMobile(lang, patientPhone, hospitalId , ref errStatus, ref errMessage);
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

        [HttpPost]
        [Route("v2/login-mobile")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientMobileCheck(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;
                if (!string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);
                    
                    var PCell = col["patient_phone"];

                    var loginDb = new Login2DB();
                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    var OTP = "";

                    loginDb.CheckLoginUser_Mobile(lang, hospitalId, PCell, ref errStatus, ref errMessage, ref OTP);

                    if (errStatus == 1)
                    {
                        resp.status = errStatus;
                        resp.status = 1;
                        //string smsRes = "";
                        var PhoneNumber = PCell;
                        string MsgContent = "";
                        //MsgContent = "<#> SGHC OTP Code " + OTP + " ";
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();




                        if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                            Util.SendTestSMS(PhoneNumber, MsgContent);
                        else if (hospitalId == 201)
                            Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                        else if (Util.UaeBranches.Contains(hospitalId))
                        {
                            string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                            log.Info("UAE SMS Reponse: " + response);
                        }
                        resp.msg = errMessage;                        
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
            catch (Exception ex)
            {

                log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/Resend-otp")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Resend_OTP(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {

                var lang = "EN";
                var hospitalId = 0;
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["OTP_TYPE"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var PhoneNumber = col["patient_phone"]; 
                    var patientMrn = col["patient_reg_no"];
                    var OTP_TYPE = Convert.ToInt32(col["OTP_TYPE"]);

                    var loginDb = new Login2DB();
                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    var OTP = "";

                    OTP = loginDb.GetVerificaitonCode2Resend(hospitalId , patientMrn , PhoneNumber, OTP_TYPE);
                    if (OTP != "")
                    {
                        resp.status = errStatus;
                        if (errStatus == 0)
                        {
                            resp.status = 1;
                            string smsRes = "";                            

                            string MsgContent = "";                            
                            MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                            MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();

                            if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                                Util.SendTestSMS(PhoneNumber, MsgContent);
                            else if (hospitalId == 201)
                                Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                            else if (Util.UaeBranches.Contains(hospitalId))
                            {
                                string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                                log.Info("UAE SMS Reponse: " + response);
                            }
                        }
                        resp.msg = "OTP has been resent to your registered number.";
                        //resp.response = userInfo;
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
            catch (Exception ex)
            {

                log.Error(ex);
            }

            return Ok();
        }






        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}