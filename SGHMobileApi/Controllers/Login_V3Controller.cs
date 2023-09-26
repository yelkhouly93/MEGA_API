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
    public class Login_V3Controller : ApiController
    {
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


    }
}
