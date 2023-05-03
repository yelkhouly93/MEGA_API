using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Common;
using SGHMobileApi.Extension;
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();
        /// <summary>
        /// Check patient data by passing patient id or phone number.
        /// </summary>
        /// <returns>Return list of patient and his/her reservation history</returns>
        [HttpPost]
        [Route("patient")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientData(FormDataCollection col)
        {   
            try
            {
                _resp = new GenericResponse();

                if (!string.IsNullOrEmpty(col["hospital_id"]) && (!string.IsNullOrEmpty(col["patient_id"]) || !string.IsNullOrEmpty(col["patient_phone"])))
                {
                    var lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientId = Convert.ToInt32(col["patient_id"]);
                    var patientPhone = col["patient_phone"];
                    
                    
                    PatientData patientData;
                    var apiCaller = new PatientApiCaller();

                    var errStatus = 0;
                    var errMessage = "";

                    if (!Util.OasisBranches.Contains(hospitalId))
                    {
                        _patientDb = new PatientDB();
                        patientData = _patientDb.CheckPatientData(lang, hospitalId, patientId, patientPhone, null, ref errStatus, ref errMessage);
                    }
                    else
                    {
                        Log.Info("PatientId : " + patientId);
                        Log.Info("HospitalId : " + hospitalId);
                        patientData = apiCaller.CheckPatientData(lang, hospitalId, patientId, patientPhone, null, ref errStatus, ref errMessage);
                    }
                    

                    if (patientData != null)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = patientData;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Fail";
                    }

                    return Ok(_resp);
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                }
            }
            catch (Exception ex)
            {
                _resp.status = 0;
                _resp.msg = ex.ToString();
                Log.Error(ex);

            }
            return Ok(_resp);
        }

        
        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        [HttpPost]
        [Route("patient-diagnosis-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPateintDiagnosis(FormDataCollection col)
        {
            try
            {
                string errMessage = "";
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) )
                {
                    var lang = col["lang"];
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    List<PatientDiagnosis> allPatientDiagnosis;
                    var apiCaller = new PatientDiagnosisApiCaller();
                    var errStatus = 0;
                    if (hospitalId != 10)
                    {
                        PatientDB patientDb = new PatientDB();
                        allPatientDiagnosis = patientDb.GetPatientDiagnosis(lang, hospitalId, registrationNo, ref errStatus, ref errMessage);
                    }
                    else
                    {
                        allPatientDiagnosis = apiCaller.GetPatientDiagnosis(lang, hospitalId, registrationNo, ref errStatus, ref errMessage);
                    }

                    if (allPatientDiagnosis != null && allPatientDiagnosis.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = errMessage;
                        _resp.response = allPatientDiagnosis;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = errMessage;
                    }

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception ex)
            {
                _resp.status = 0;
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }
            
            return Ok(_resp);
        }



        [HttpPost]
        [Route("patient-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration(FormDataCollection col)
        {
            var registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;

            registerPatient.Lang = col["lang"];
            registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);
            registerPatient.PatientTitleId = 0; // Convert.ToInt32(col["patient_title_id"]);
            registerPatient.PatientFirstName = col["patient_first_name"];
            registerPatient.PatientMiddleName = col["patient_middle_name"];
            registerPatient.PatientLastName = col["patient_last_name"];
            registerPatient.PatientFamilyName = null; // col["patient_family_name"];
            registerPatient.PatientPhone = col["patient_phone"];
            registerPatient.PatientEmail = null; // col["patient_email"];
            registerPatient.PatientNationalId = col["patient_national_id"];
            registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);
            registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
            registerPatient.PatientAddress = null; // col["patient_address"];
            registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
            registerPatient.PatientMaritalStatusId = 0; // Convert.ToInt32(col["patient_marital_status_id"]);
            //registerPatient.skipDuplicateCheck = Convert.ToBoolean(col["skip_duplicate_check"]);
            registerPatient.skipDuplicateCheck = false;

            var apiCaller = new NewPatientRegistrationApiCaller();

            var status = 0;
            var msg = "";
            var successType = "";
            var registrationNo = 0;
            var activationNo = 0;
            var errorType = "";

            if (!Util.OasisBranches.Contains(registerPatient.HospitaId))
            {
                var patientDb = new PatientDB();
                registerPatientResFailure = patientDb.RegisterNewPatient(registerPatient, ref status, ref msg, ref errorType, ref successType, ref registrationNo, ref activationNo);
            }
            else
            {
                registerPatientResFailure = apiCaller.RegisterNewPatient(registerPatient, ref status, ref msg, ref errorType, ref successType, ref registrationNo, ref activationNo);
            }


            RegisterPatientResponse resp = new RegisterPatientResponse();

            if (status == 1)
            {
                string PName = (registerPatient.PatientFirstName == null ? "" : registerPatient.PatientFirstName + " ") +
                    (registerPatient.PatientMiddleName == null ? "" : registerPatient.PatientMiddleName + " ") +
                    (registerPatient.PatientLastName == null ? "" : registerPatient.PatientLastName);
                string branchName = (registerPatient.HospitaId == 1 ? "Jeddah" : (registerPatient.HospitaId == 2 ? "Riyadh" : (registerPatient.HospitaId == 3 ? "Madinah" : (registerPatient.HospitaId == 10 ? "Dammam" : (registerPatient.HospitaId == 8 ? "Hail" : (registerPatient.HospitaId == 101 ? "Beverly" : (registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));

                string smsRes = "";

                if (registerPatient.HospitaId != 201 && !Util.OasisBranches.Contains(registerPatient.HospitaId))
                    smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                else if (registerPatient.HospitaId == 201)
                {
                    registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                    smsRes = Util.SendSMS_Cairo(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                }
                else if (Util.OasisBranches.Contains(registerPatient.HospitaId))
                {

                    registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                    smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Please contact with hospital to get your File No.");
                }

                //string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Your new File No. for selected SGH Branch is : " + registrationNo + ".\nUse this OTP to login SGH Mobile App: " + activationNo);
                resp.activation_num = registerPatient.HospitaId == 10 ? 9114 : activationNo;
                resp.msg = registerPatient.HospitaId == 10 ? "Thank you " + PName + " for opening a file with SGH " + branchName + ". Please contact with hospital to get your File No." : msg;
                resp.response = new RegistePatientResponseSuccess() { registration_no = registerPatient.HospitaId == 10 ? 155623 : registrationNo, national_id = registerPatient.PatientNationalId, phone = registerPatient.PatientPhone, name = null, name_ar = null };
                resp.smsResponse = smsRes;
                resp.status = 1;
                resp.success_type = successType;

                Log.Info("\n" + resp.msg);
                Log.Info("\n" + resp.activation_num);
                Log.Info("\n" + resp.smsResponse);

            }
            else
            {
                resp.error_type = errorType;
                resp.msg = msg;
                resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }



        [HttpPost]
        [Route("patientDependent-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientDependent(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["relationship"]) && col["MRN"] != "0" && !string.IsNullOrEmpty(col["National_ID"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["registrationNo"]);
                var nationalId = col["National_ID"];
                var relation = col["relationship"];
                var status = 0;
                var msg = "";
                
                var patientDb = new PatientDB();
                var dt = patientDb.GetDependentPatientDataTable(hospitalId, nationalId, registrationNo, relation,ref status , ref msg);

                if (dt != null && dt.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = dt;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        [HttpPost]
        [Route("patient-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;
                if (string.IsNullOrEmpty(col["Source"]))
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter- Source missing";
                    return Ok(resp);
                }
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    //if (!string.IsNullOrEmpty(col["Source"]))
                        var Source = col["Source"];


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

                    var userInfo = loginDb.ValidateLoginUser_List(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, ref OTP , false);

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

                            if (hospitalId != 201 && !Util.UaeBranches.Contains(hospitalId))
                                Util.SendTestSMS(PhoneNumber, MsgContent);
                            else if (hospitalId == 201)
                                Util.SendSMS_Cairo(PhoneNumber, MsgContent);
                            else if (Util.UaeBranches.Contains(hospitalId))
                            {
                                string response = Util.SendSMS_UAE(hospitalId, PhoneNumber, MsgContent);
                                Log.Info("UAE SMS Reponse: " + response);
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
            catch (Exception ex)
            {

                Log.Error(ex);
            }

            return Ok();
        }



        

        [HttpPost]
        [Route("patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientGET(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["ClientID"]))
                {
                    if (!Util.CheckCallingAccess(col["ClientID"].ToString(), "patient-get"))
                    {
                        resp.status = 0;
                        resp.msg = "Access Denied.";
                        return Ok(resp);

                    }

                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    
                    var errStatus = 0;
                    var errMessage = "";

                    PatientDB patientDb = new PatientDB();
                    var dt = patientDb.GetPatientDataDT(lang, hospitalId, patientMrn, ref errStatus,ref errMessage);

                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = dt;                        
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

                Log.Error(ex);
            }

            return Ok();
        }

        //[HttpGet]
        //[Route("api/DynamicApi-test-V2")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult DynamicApi(FormDataCollection col)
        //{
        //    try
        //    {
        //        // first Get the Store Procedure Name from API
        //        // 
        //        GenericResponse resp = new GenericResponse();
        //        var spName = col["sp_Name"];
        //        if (spName != null)
        //        {
        //            var lang = col["lang"];
        //            var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            var clinicId = Convert.ToInt32(col["clinic_id"]);
        //            var pageno = Convert.ToInt32(col["page_no"]);
        //            var pagesize = Convert.ToInt32(col["page_size"]);

        //            //DBO.[Get_Doctors_SP]

        //            List<dynamic> expandoList = new List<dynamic>();

        //            //CommentFor Testing //if (loginDb.ValidateOTP(regno, hospitalId, otpid))  // Validate OTP First 
        //            if (true) // OTP Success
        //            {
        //                CommonDB dbCommonDb = new CommonDB();
        //                var dt = dbCommonDb.GetDataTable(spName, hospitalId, clinicId);

        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    //create a new ExpandoObject() at each row
        //                    var expandoDict = new ExpandoObject() as IDictionary<String, Object>;
        //                    foreach (DataColumn coll in dt.Columns)
        //                    {
        //                        //put every column of this row into the new dictionary
        //                        expandoDict.Add(coll.ToString(), row[coll.ColumnName].ToString());
        //                    }

        //                    //add this "row" to the list
        //                    expandoList.Add(expandoDict);
        //                }


        //                if (expandoList.Count > 0)
        //                {
        //                    resp.status = 1;
        //                    resp.msg = "errMessage";
        //                    resp.response = expandoList;
        //                }
        //                else
        //                {
        //                    resp.status = 0;
        //                    resp.msg = "errMessage";
        //                    resp.response = null;
        //                    resp.error_type = "errStatus.ToString();";
        //                }
        //                return Ok(resp);
        //            }
        //            else
        //            {
        //                // OTP Fail condition                    
        //                resp.status = 0;
        //                resp.msg = "OTP Not Match, Please enter the Correct OTP";
        //                resp.response = null;
        //                resp.error_type = "OTP Miss-match";
        //                return Ok(resp);
        //            }
        //        }
        //        else
        //        {
        //            // no SP Name Found 
        //            resp.status = 0;
        //            resp.msg = "SP Parameter missing";
        //            resp.response = null;
        //            resp.error_type = "SP Parameter missing";
        //            return Ok();
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}

        [HttpPost]
        [Route("v2/patient-check")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientList_Request_v2(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;
                var Source = "";
                var isEncrypt = true;
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);

                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    var PatientNationId = col["patient_national_id"];
                    var PCell = col["patient_phone"];


                    if (!string.IsNullOrEmpty(col["Source"]))
                        Source = col["Source"];

                    if (!string.IsNullOrEmpty(col["IsEncrypt"]))
                    {
                        if (col["IsEncrypt"] == "0")
                            isEncrypt = false;

                    }
                        


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

                    var userInfo = loginDb.ValidateLoginUser_List(lang, hospitalId, PCell, PatientNationId, patientMrn, Source, ref errStatus, ref errMessage, ref OTP, isEncrypt);

                    if (errStatus != 1)
                    {
                        resp.status = errStatus;
                        if (errStatus == 0)
                        {

                            resp.status = 1;
                            //string smsRes = "";
                            DataRow dr = userInfo.Rows[0];
                            var PhoneNumber = dr["PatientCellNo2"].ToString();
                            //userInfo.Rows[0]["PatientCellNo2"] = "";

                            //PhoneNumber = "0592285955";

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
                                Log.Info("UAE SMS Reponse: " + response);
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
            catch (Exception ex)
            {

                Log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientGET_v2(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) )
                {
                    //if (!Util.CheckCallingAccess(col["ClientID"].ToString(), "patient-get"))
                    //{
                    //    resp.status = 0;
                    //    resp.msg = "Access Denied.";
                    //    return Ok(resp);

                    //}

                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    var errStatus = 0;
                    var errMessage = "";

                    PatientDB patientDb = new PatientDB();
                    // Change as per Mehmode Request
                    //var dt = patientDb.GetPatientDataDT_V2(lang, hospitalId, patientMrn, ref errStatus, ref errMessage);
                    
                    var loginDb = new Login2DB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    var userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, col["patient_reg_no"].ToString(), null, ref errStatus, ref errMessage , ApiSource);


                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        //resp.response = dt;
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
            catch (Exception ex)
            {

                Log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/patient-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration_V2(FormDataCollection col)
        {
            var registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;
            RegisterPatientResponse resp = new RegisterPatientResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_first_name"]) && !string.IsNullOrEmpty(col["patient_last_name"])
                && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_birthday"]) 
                && !string.IsNullOrEmpty(col["patient_gender"]) && !string.IsNullOrEmpty(col["patient_nationality_id"]) && !string.IsNullOrEmpty(col["patient_marital_status_id"]))
            {

                var checkdateTime = Convert.ToDateTime(col["patient_birthday"]);
                var YEARPART = checkdateTime.Year;
                 if (YEARPART <= 1900)
                {
                    resp.status = 0;
                    resp.msg = "Birth Date should be in Gregorian : -- ";
                    return Ok(resp);
                }


                try
                {
                    registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);

                    registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);
                 

                    registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
                    registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                    registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
                }
                catch (Exception e)
                {
                    resp.status = 0;
                    resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(resp);
                }


                //var Sources = "MobileApp";
                var Sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];


                registerPatient.Lang = col["lang"];

                if (!string.IsNullOrEmpty(col["patient_middle_name"]))
                {
                    registerPatient.PatientMiddleName = col["patient_middle_name"];
                }
                else
                    registerPatient.PatientMiddleName = "";




                registerPatient.PatientTitleId = 0; // Convert.ToInt32(col["patient_title_id"]);
                registerPatient.PatientFirstName = col["patient_first_name"];
                
                registerPatient.PatientLastName = col["patient_last_name"];
                registerPatient.PatientFamilyName = null; // col["patient_family_name"];
                registerPatient.PatientPhone = col["patient_phone"];
                registerPatient.PatientEmail = null; // col["patient_email"];
                registerPatient.PatientNationalId = col["patient_national_id"];
                
                
                registerPatient.PatientAddress = null; // col["patient_address"];
                
                //registerPatient.PatientMaritalStatusId = 0; // Convert.ToInt32(col["patient_marital_status_id"]);
                
                registerPatient.skipDuplicateCheck = false;
                //registerPatient.PatientNationalityId = Convert.ToInt32(col["Nationality_id"]);

                var Patient_Pwd = "";

                if (!string.IsNullOrEmpty(col["Patient_Pwd"]))
                    Patient_Pwd = col["Patient_Pwd"];

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";
                var successType = "";
                var registrationNo = 0;
                var activationNo = 0;
                var errorType = "";

                var patientDb = new PatientDB();
                registerPatientResFailure = patientDb.RegisterNewPatient_V2(registerPatient, ref status, ref msg, ref errorType, ref successType, ref registrationNo, ref activationNo , Sources);





                if (status == 1)
                {
                    /*******
                     ******* New Changes 06-July-2022 
                     ******* Save Patient Password on Successful registration
                     ********/
                    var PWDstatus = 0;
                    var PWDmsg = "";
                    if (Patient_Pwd != "")
                    {
                        patientDb.Save_PatientPwd(lang, registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, Patient_Pwd, ref PWDstatus, ref PWDmsg);
                    }


                    //add LOGS
                    patientDb.AddPatient_Log(registerPatient.HospitaId ,registrationNo, registerPatient.PatientNationalId , registerPatient.PatientFirstName , registerPatient.PatientLastName, registerPatient.PatientPhone, Sources);



                    string PName = (registerPatient.PatientFirstName == null ? "" : registerPatient.PatientFirstName + " ") +
                        (registerPatient.PatientMiddleName == null ? "" : registerPatient.PatientMiddleName + " ") +
                        (registerPatient.PatientLastName == null ? "" : registerPatient.PatientLastName);
                    
                    //string branchName = (registerPatient.HospitaId == 1 ? "Jeddah" : (registerPatient.HospitaId == 2 ? "Riyadh" : (registerPatient.HospitaId == 3 ? "Madinah" : (registerPatient.HospitaId == 10 ? "Dammam" : (registerPatient.HospitaId == 8 ? "Hail" : (registerPatient.HospitaId == 101 ? "Beverly" : (registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));

                    string branchName = "Sadui German Health";

                    branchName = patientDb.GetBranchName(registerPatient.HospitaId);
                    if (string.IsNullOrEmpty(branchName))
                        branchName = "Sadui German Health";

                    string smsRes = "";

                    if (registerPatient.HospitaId != 201 && !Util.OasisBranches.Contains(registerPatient.HospitaId))
                        smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                    else if (registerPatient.HospitaId == 201)
                    {
                        registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                        smsRes = Util.SendSMS_Cairo(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                    }
                    else if (Util.OasisBranches.Contains(registerPatient.HospitaId))
                    {

                        registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                        smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with  " + branchName + ". Please contact with hospital to get your File No.");
                    }

                    //string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Your new File No. for selected SGH Branch is : " + registrationNo + ".\nUse this OTP to login SGH Mobile App: " + activationNo);
                    var tempPatient_Name = registerPatient.PatientFirstName + ' ' + registerPatient.PatientMiddleName + ' ' + registerPatient.PatientLastName;
                    resp.activation_num = registerPatient.HospitaId == 10 ? 9114 : activationNo;
                    resp.msg = registerPatient.HospitaId == 10 ? "Thank you " + PName + " for opening a file with  " + branchName + ". Please contact with hospital to get your File No." : msg;
                    resp.response = new RegistePatientResponseSuccess() { registration_no = registerPatient.HospitaId == 10 ? 155623 : registrationNo, national_id = registerPatient.PatientNationalId, phone = registerPatient.PatientPhone, name = tempPatient_Name, name_ar = tempPatient_Name };
                    resp.smsResponse = smsRes;
                    resp.status = 1;
                    resp.success_type = successType;

                    Log.Info("\n" + resp.msg);
                    Log.Info("\n" + resp.activation_num);
                    Log.Info("\n" + resp.smsResponse);

                }
                else
                {
                    resp.error_type = errorType;
                    resp.msg = msg;
                    resp.response = registerPatientResFailure;
                    resp.status = 0;
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed! Missing Parameters";
            }
            

            return Ok(resp);
        }



        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        [HttpPost]
        [Route("v2/patient-diagnosis-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPateintDiagnosis_v2(FormDataCollection col)
        {
            try
            {
                string errMessage = "";
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    var lang = col["lang"];
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    List<PatientDiagnosis> allPatientDiagnosis;
                    var apiCaller = new PatientDiagnosisApiCaller();
                    var errStatus = 0;
                    PatientDB patientDb = new PatientDB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    allPatientDiagnosis = patientDb.GetPatientDiagnosis(lang, hospitalId, registrationNo, ref errStatus, ref errMessage , ApiSource);


                    if (allPatientDiagnosis != null && allPatientDiagnosis.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = errMessage;
                        _resp.response = allPatientDiagnosis;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = errMessage;
                    }

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception ex)
            {
                _resp.status = 0;
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }

            return Ok(_resp);
        }

        
        [HttpPost]
        [Route("v2/patient-visits-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientVisits(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);


                var allAppointmnetList = patientDb.GetPatientVisits(lang, hospitalId, registrationNo);


                if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allAppointmnetList;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        [HttpPost]
        [Route("v2/patient-prescription-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientPrescription(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = col["lang"];
                var hospitaId = 0;
                var registrationNo = 0;
                int errStatus = 0;
                string errMessage = "";
                PatientDB _patientDB = new PatientDB();

                try
                {                    
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();


                var _allPatientMedDT = _patientDB.GetPatientPrescriptionDT(lang, hospitaId, registrationNo, ref errStatus, ref errMessage , ApiSource);
                                

                if (_allPatientMedDT != null && _allPatientMedDT.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.response = _allPatientMedDT;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = errMessage;
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }
            


            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/PatientFamily-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientFamilyList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;

                // For Now Only Mobily Number 
                //if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
                if (!string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);

                    
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    var PatientNationId = col["patient_national_id"];

                    var PCell = col["patient_phone"];

                    
                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    //var OTP = "";

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
                    PatientDB _patientDB = new PatientDB();

                    var PatientFamilyDT = _patientDB.GetPatientFamily_List(lang, hospitalId, PCell, PatientNationId, patientMrn, ref errStatus, ref errMessage);

                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = PatientFamilyDT;
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
                Console.WriteLine(ex);
                //log.Error(ex);
            }

            return Ok();
        }



        [HttpPost]
        [Route("v2/PatientOrder-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientOrder(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;

                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) )
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);


                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    var VisitorID = 0;
                    if (string.IsNullOrEmpty(col["visit_id"]))
                        VisitorID = 0;
                    else
                        VisitorID = Convert.ToInt32(col["visit_id"]);

                    //var errStatus = 0;
                    //var errMessage = "";

                    PatientDB _patientDB = new PatientDB();
                    var PatientOrderDT = _patientDB.GetPatientOrder_List(lang, hospitalId,patientMrn, VisitorID);

                    if (PatientOrderDT != null && PatientOrderDT.Rows.Count >  0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientOrderDT;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
                        
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
                var test = ex;                
            }

            return Ok();
        }



        [HttpPost]
        [Route("v2/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo(FormDataCollection col)
        {
            var resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "";
            //try
            //{
            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
                {   
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    int errStatus = 0;
                    string errMessage = "";

                    PatientDB _patientDB = new PatientDB();
                    var PatientInsuranceDT = _patientDB.GetPatientInsuranceInfo_DT(hospitalId, patientMrn,ref errStatus , ref errMessage);


                    if (PatientInsuranceDT != null && PatientInsuranceDT.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientInsuranceDT;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";

                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            //}
            //catch (Exception ex)
            //{
            //    var test = ex;
            //}

            //return Ok();
        }



        //[HttpPost]
        //[Route("v2/LatestPrescription-get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PatientLatestPrescriptions_Copy(FormDataCollection col)
        //{
        //    var resp = new GenericResponse();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Order_ID"]))
        //        {
        //            var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
        //            var Lang= "EN";
        //            if (!string.IsNullOrEmpty(col["Lang"]))
        //                Lang = col["Lang"];

        //            // Not the Order_ID  prameter = Visit Id 
        //            var OrderID = Convert.ToInt32(col["Order_ID"]);

        //            var VisitID = "Order_ID=" + OrderID.ToString() + "&MRN=" + patientMrn + "&BranchId=" + hospitalId + "&Lang=" + Lang;

        //            var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
        //            var FinalURL = ConfigurationManager.AppSettings["MEDPerscriptionURLPDF"].ToString() + ParmEnc;


        //            var FinalURLNEW = Util.ConvertURL_to_PDF(FinalURL, OrderID.ToString());

        //            MedicalPrescriptions ObjReturn = new MedicalPrescriptions();

        //            ObjReturn.PdfURL = FinalURLNEW;

        //            if (ObjReturn != null )
        //            {
        //                resp.status = 1;
        //                resp.msg = "Record(s) Found";
        //                resp.response = ObjReturn;
        //            }
        //            else
        //            {
        //                resp.status = 0;
        //                resp.msg = "No Record Found";
        //            }
        //        }
        //        else
        //        {
        //            resp.status = 0;
        //            resp.msg = "Missing Parameter!";
        //        }
        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        var test = ex;
        //    }

        //    return Ok();
        //}

        [HttpPost]
        [Route("v2/Patient-LatestPrescription-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientLatestPrescriptions(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Order_ID"]))
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["Lang"]))
                        Lang = col["Lang"];

                    // Not the Order_ID  prameter = Visit Id 
                    var OrderID = Convert.ToInt32(col["Order_ID"]);

                    var VisitID = "Order_ID=" + OrderID.ToString() + "&MRN=" + patientMrn + "&BranchId=" + hospitalId + "&Lang=" + Lang;

                    var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                    var FinalURL = ConfigurationManager.AppSettings["MEDPerscriptionURLPDF"].ToString() + ParmEnc;


                    var FinalURLNEW = Util.ConvertURL_to_PDF(FinalURL, OrderID.ToString());

                    MedicalPrescriptions ObjReturn = new MedicalPrescriptions();

                    ObjReturn.PdfURL = FinalURLNEW;

                    if (ObjReturn != null)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = ObjReturn;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
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
                var test = ex;
            }

            return Ok();
        }



        [HttpPost]
        [Route("v2/Patient-Search-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientSearchList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;

                
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))                
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);

                    //int patientMrn;
                    //if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                        var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    //string PatientNationId;
                    //if (!string.IsNullOrEmpty(col["patient_national_id"]))
                        var PatientNationId = col["patient_national_id"];


                    //string PCell;
                    //if (!string.IsNullOrEmpty(col["patient_phone"]))
                     var   PCell = col["patient_phone"];
                    


                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    //var OTP = "";

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
                        PatientNationId = col["patient_national_id"];
                        if (PatientNationId.Length != 10)
                        {
                            resp.status = 0;
                            resp.msg = "Wrong input! Invalid National ID";
                            resp.error_type = errStatus.ToString();
                            return Ok(resp);
                        }
                    }
                    PatientDB _patientDB = new PatientDB();



                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    var PatientFamilyDT = _patientDB.GetPatientSearch_List(lang, hospitalId, PCell, PatientNationId, patientMrn, ref errStatus, ref errMessage , ApiSource);

                    if (errStatus != 1)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = PatientFamilyDT;
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
                Console.WriteLine(ex);
                //log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/BookingPatientFamily-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetBookingPatientFamilyList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                

                
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Booking_hospital_id"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var BookinghospitalId = Convert.ToInt32(col["Booking_hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);                    
                    var errStatus = 0;
                    var errMessage = "";                    
                    
                    PatientDB _patientDB = new PatientDB();
                    var PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, hospitalId,patientMrn, BookinghospitalId, ref errStatus, ref errMessage);

                    if (errStatus == 1)
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        resp.error_type = "0";
                        
                    }
                    else
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = PatientFamilyDT;
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
                Console.WriteLine(ex);
                //log.Error(ex);
            }

            return Ok();
        }


        /// <summary>
        /// Get all the bills of patient against his/her registration no.
        /// </summary>
        /// <returns>Return the list of bills for patient.</returns>
        [HttpPost]
        [Route("v2/patient-bills-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostGetPatientBills(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
                {

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    int errStatus = 0;
                    string errMessage = "";

                    string consentMessage = string.Empty;
                    var _bills = _patientDb.GetPatientBills(hospitaId, registrationNo, ref errStatus, ref errMessage);

                    if (_bills != null && _bills.Rows.Count > 0)
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = _bills;
                    }
                    else
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = null;
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
                Console.WriteLine(ex);
                //log.Error(ex);

            }
            return Ok();
        }




        [HttpPost]
        [Route("v2/Patient-BasicData-update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Update_PatientBasicData(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["patient_DOB"]) && !string.IsNullOrEmpty(col["Sources"]))
            {
                var Status = 0;
                var Msg = "";

                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var PatientPhone = col["patient_phone"];
                var patient_DOB = col["patient_DOB"];                
                var patient_Gender = col["patient_Gender"];
                var Marital_Status = col["Marital_Status"];
                var PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                var EMail = col["patient_Email"];
                var Sources = col["Sources"];

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

                var patient_Gender_i = 0;
                if (!string.IsNullOrEmpty(patient_Gender))
                {
                    if (patient_Gender != "F" && patient_Gender != "M")
                    {
                        _resp.status = 0;
                        _resp.msg = "Failed : Wrong Gender Format";
                        return Ok(_resp);
                    }
                    if (patient_Gender == "F")
                        patient_Gender_i = 1;
                    else
                        patient_Gender_i = 2;
                }
                
                var iMarital_Status = 0;
                if (!string.IsNullOrEmpty(Marital_Status))
                {
                    if (Marital_Status != "S" && Marital_Status != "M")
                    {
                        _resp.status = 0;
                        _resp.msg = "Failed : Wrong Marital Status Format";
                        return Ok(_resp);
                    }

                    if (Marital_Status == "S")
                        iMarital_Status = 6;

                    if (Marital_Status == "M")
                        iMarital_Status = 2;
                }

                PatientDB patientDb = new PatientDB();
                var RegistrationNo = Convert.ToInt32(CMRN);
                patientDb.UpdatePatientData(hospitaId, RegistrationNo, PDateOfBirth, patient_Gender_i, PatientPhone, iMarital_Status, PatientNationalityId, EMail, Sources, ref Status, ref Msg);
                _resp.status = Status;
                _resp.msg = Msg;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        [HttpPost]
        [Route("v2/Patient-pwd-update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SaveUpdate_PatientPwd(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["patient_national_id"])
                && !string.IsNullOrEmpty(col["patient_Pwd"]))
            {
                var Status = 0;
                var Msg = "";

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var PatientNationalalID = col["patient_national_id"];
                var patient_pwd = col["patient_Pwd"];
                PatientDB patientDb = new PatientDB();
                var RegistrationNo = Convert.ToInt32(CMRN);

                if (!string.IsNullOrEmpty(col["patient_national_id"]))
                {
                    if (PatientNationalalID.Length != 10 || !IsDigitsOnly(PatientNationalalID))
                    {
                        _resp.status = 0;
                        _resp.msg = "Wrong Format! Invalid National ID Number.";
                        _resp.error_type = "0";
                        return Ok(_resp);
                    }
                }

                patientDb.Save_PatientPwd(lang, hospitaId, RegistrationNo, PatientNationalalID , patient_pwd, ref Status, ref Msg);
                _resp.status = Status;
                _resp.msg = Msg;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

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
        //var PWDstatus = 0;
        //var PWDmsg = "";
        //patientDb.Save_PatientPwd(lang, registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, Patient_Pwd, ref PWDstatus, ref PWDmsg);

    }
}